using AstroMath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AstroImage
{
    public class FitsFile
    {
        byte[] headerRecord = new byte[80];
        byte[] dataUnit = new byte[2880];
        int bCount;

        public int[] FITS_Vector = new int[1];
        public UInt16[,] FITS_Array;

        private List<string> fitsHdr = new List<string>();
        const int ImageHeaderLength = 56 + (256 * 4);

        public string FilePath { get; set; }
        public string ObjectName { get; set; }
        public int Xaxis { get; set; }  //Pixels
        public int Yaxis { get; set; }  //Pixels
        public double ObjectRA { get; set; }  //OBJCTRA in Hours
        public double ObjectDec { get; set; }  //OBJCTDEC in Degrees
        public double PA { get; set; }  //ORIENTAT in Degrees
        public double XpixSz { get; set; } //XPIXSZ in microns
        public double YpixSz { get; set; }  //YPIXSZ in microns
        public double FocalLength { get; set; }  //FOCALLEN in mm
        public double PixelScale { get; set; }  //arcsec per pixel
        public double CRVAL_RA { get; set; } //Image center RA in Degrees
        public double CRVAL_Dec { get; set; }  //Image center Dec in Degrees
        public double CRPIX_X { get; set; } //Image center X in pixels
        public double CRPIX_Y { get; set; }  //Image center Y in pixels
        public double CD1_1 { get; set; }
        public double CD1_2 { get; set; }
        public double CD2_1 { get; set; }
        public double CD2_2 { get; set; }
        public int MaxValue => FITS_Vector.Max();
        public int MinValue => FITS_Vector.Min();
        public int AvgValue => (int)FITS_Vector.Average();

        public FitsFile() { } //null instance

        public FitsFile(FitsFile fs)  //make a copy
        {
            FITS_Array = new UInt16[fs.Xaxis, fs.Yaxis];
            FITS_Vector = new int[fs.Xaxis * fs.Yaxis];
            Xaxis = fs.Xaxis;
            Yaxis = fs.Yaxis;
            ObjectRA = fs.ObjectRA;
            ObjectDec = fs.ObjectDec;
            PA = fs.PA;
            XpixSz = fs.XpixSz;
            YpixSz = fs.YpixSz;
            FocalLength = fs.FocalLength;
            PixelScale = fs.PixelScale;
            CD1_1 = fs.CD1_1;
            CD1_2 = fs.CD1_2;
            CD2_1 = fs.CD2_1;
            CD2_2 = fs.CD2_2;
            CRVAL_RA = fs.CRVAL_RA;
            CRVAL_Dec = fs.CRVAL_Dec;
            CRPIX_X = fs.CRPIX_X;
            CRPIX_Y = fs.CRPIX_Y;

            for (int i = 0; i < FITS_Vector.Length; i++)
            {
                FITS_Vector[i] = fs.FITS_Vector[i];
                //FITS_Hist[(int)(FITS_Vector[i] / FITS_Hist_BucketWidth)] += 1;
            }
            for (int iy = 0; iy < Yaxis; iy++)
            {
                for (int ix = 0; ix < Xaxis; ix++)
                {
                    FITS_Array[ix, iy] = (ushort)FITS_Vector[(ix) + (iy * Xaxis)];
                }
            }
        }

        public FitsFile(string filepath, bool dataswitch)
        {

            //Opens file set by filepath (assumes it's a FITS formatted file)
            //Reads in header in 80 character strings, while ("END" is found
            //if (dataswitch is true, { an array "FITSimage" is created and populated with the FITS image data as an class Image
            //  otherwise just the header info is retained
            //
            FilePath = filepath;

            int keyindex = 0;
            FileStream FitsHandle = File.OpenRead(filepath);
            do
            {
                bCount = FitsHandle.Read(headerRecord, 0, 80);
                keyindex += 80;
                //Check for empty file (file error on creation), just opt out if (so
                if (bCount == 0)
                {
                    return;
                }
                fitsHdr.Add(System.Text.Encoding.ASCII.GetString(headerRecord));
            } while (!fitsHdr.Last().StartsWith("END "));
            //Continue through any remaining header padding
            while ((keyindex % 2880) != 0)
            {
                bCount = FitsHandle.Read(headerRecord, 0, 80);
                keyindex += 80;
            };
            ObjectName = ReadKey("OBJECT");

            //Get the array dimensions
            int bitpix = Convert.ToInt32(ReadKey("BITPIX"));
            Xaxis = Convert.ToInt32(ReadKey("NAXIS1"));
            Yaxis = Convert.ToInt32(ReadKey("NAXIS2"));
            ObjectRA = FitsKeytoHours(ReadKey("OBJCTRA"));
            ObjectDec = FitsDegtoDegrees(ReadKey("OBJCTDEC"));
            CD1_1 = Convert.ToDouble(ReadKey("CD1_1"));
            CD1_2 = Convert.ToDouble(ReadKey("CD1_2"));
            CD2_1 = Convert.ToDouble(ReadKey("CD2_1"));
            CD2_2 = Convert.ToDouble(ReadKey("CD2_2"));
            CRVAL_RA = Convert.ToDouble(ReadKey("CRVAL1"));
            CRVAL_Dec = Convert.ToDouble(ReadKey("CRVAL2"));
            CRPIX_X = Convert.ToDouble(ReadKey("CRPIX1"));
            CRPIX_Y = Convert.ToDouble(ReadKey("CRPIX2"));

            PA = RotationToPA(CD1_1, CD1_2, CD2_1, CD2_2);

            //XpixSz = 0.72;
            // Ypixsz = 0.72;
            string sXpixsize = ReadKey("XPIXSZ");
            string sYpixsize = ReadKey("YPIXSZ");
            string sFoclen = ReadKey("FOCALLEN");

            if (sXpixsize != null) XpixSz = Convert.ToDouble(sXpixsize.TrimEnd('.'));
            if (sYpixsize != null) YpixSz = Convert.ToDouble(sYpixsize.TrimEnd('.'));
            if (sFoclen != null) FocalLength = Convert.ToDouble(sFoclen.TrimEnd('.'));

            int totalpixels = Xaxis * Yaxis;

            //int headerbytes = ImageHeaderLength;
            int totaldata = totalpixels + ImageHeaderLength;

            //Fits data section 
            if (dataswitch)
            {
                int dataindex = 0;

                int bmWidth = Xaxis;
                int bmHeight = Yaxis;
                Array.Resize(ref FITS_Vector, totalpixels);

                int bmX = 0;
                int bmY = 0;
                UInt16 bmVal;

                do
                {
                    bCount = FitsHandle.Read(dataUnit, 0, 2880);
                    for (int k = 0; k <= (bCount - 1); k += 2)
                    {
                        if (dataindex < totalpixels)
                        {
                            bmVal = TwosComplementBytesToInteger(dataUnit[k], dataUnit[k + 1]);
                            FITS_Vector[dataindex] = bmVal;
                            //FITS_Hist[(int)(bmVal / FITS_Hist_BucketWidth)] += 1;
                        }
                        dataindex += 1;
                        bmX += 1;
                        if (bmX >= bmWidth)
                        {
                            bmY += 1;
                            bmX = 0;
                        }
                    }
                } while (bCount != 0);  //No more bytes read in = done
            }
            //Generate Fits image array
            FITS_Array = new UInt16[Xaxis, Yaxis];
            for (int iy = 0; iy < Yaxis; iy++)
            {
                for (int ix = 0; ix < Xaxis; ix++)
                {
                    FITS_Array[ix, iy] = (ushort)FITS_Vector[(ix) + (iy * Xaxis)];
                }
            }
            //Close file
            FitsHandle.Close();
            return;
        }

        public string ReadKey(string keyword)
        {
            //return;s contents of key word entry, scrubbed of extraneous characters
            foreach (string keyline in fitsHdr)
            {
                if (keyline.Contains(keyword))
                {
                    int startindex = keyline.IndexOf("=");

                    int endindex = keyline.IndexOf("/");
                    if (endindex == -1)
                    {
                        endindex = keyline.Length - 1;
                    }

                    string keylineN = keyline.Substring(startindex + 1, endindex - (startindex + 1));
                    // keyline = Replace(keyline, "//", " ");
                    keylineN = keylineN.Replace('/', ' ');
                    keylineN = keylineN.Replace('\'', ' ');
                    keylineN = keylineN.Trim(' ');
                    return (keylineN);
                }
            }
            return (null);
        }

        public Point RADECtoImageXY(double hoursRA, double degreesDec)
        {
            //RA in hours, Dec in degrees
            //Return x,y point in image
            //Set resolution for x and y in arcsec per pixel
            double xResArcSec = (206.265 / FocalLength) * XpixSz;
            double yResArcSec = (206.265 / FocalLength) * YpixSz;
            double xPixPerDegree = xResArcSec * 3600;
            double yPixPerDegree = yResArcSec * 3600;
            //Get the image size and resolution and center of image in X/Y in radians
            double targetRADegrees = Transform.HoursToDegrees(Convert.ToDouble(hoursRA));
            double targetDecDegrees = degreesDec;

            //Get RA/Dec for the center of the image 
            double centerRADegrees = CRVAL_RA;
            double centerDecDegrees = CRVAL_Dec;
            //Compute unrotated xy from image center
            //Create rotation matrix
            AstroMath.Polar2D.RotationMatrix rotMat =
                new Polar2D.RotationMatrix(CD1_1 * xPixPerDegree, CD1_2 * yPixPerDegree, CD2_1 * xPixPerDegree, CD2_2 * yPixPerDegree);
            //Set relative position of target from center
            double deltaRADegrees = targetRADegrees - centerRADegrees;
            double deltaDecDegrees = targetDecDegrees - centerDecDegrees;
            //Rotate delta
            (double rotXDegrees, double rotYDegrees) = rotMat.Rotate(deltaRADegrees, deltaDecDegrees);
            double rotXPixels = rotXDegrees * xPixPerDegree;
            double rotYPixels = rotYDegrees * yPixPerDegree;
            //Offset delta x,y from center
            //Note that both X and Y are reversed
            Point deltaTargetXY = new Point(-(int)rotXPixels, -(int)rotYPixels);
            Point centerXY = new Point((int)CRPIX_X, (int)CRPIX_Y);
            Point translatedXY = new Point(centerXY.X - deltaTargetXY.X, centerXY.Y - deltaTargetXY.Y);
            //Point translatedXY = AstroMath.Polar2D.XYTranslation(centerXY, deltaTargetXY);
            return translatedXY;
        }

        public bool RotateFit180(string filepathIn)
        {
            //Invert both the rows and columns of FITS image such that the image rotates 180 degrees
            //
            //Read in the FITS header, then the data.
            //Write back out the FITS hearder, then the data, in reverse in bytes of two
            //Opens file set by filepath (assumes it//s a FITS formatted file)
            //Reads in header in 80 character strings, while ("END" is found
            //if (dataswitch is true, { an array "FITSimage" is created and populated with the FITS image data as an class Image
            //  otherwise just the header info is retained
            //
            int keyindex = -1;
            FileStream FitsHandleIn = File.OpenRead(filepathIn);
            do
            {
                keyindex++;
                bCount = FitsHandleIn.Read(headerRecord, 0, 80);
                //Check for empty file (file error on creation), just opt out if (so
                if (bCount == 0)
                {
                    FitsHandleIn.Close();
                    return false;
                }
                fitsHdr.Add(System.Text.Encoding.ASCII.GetString(headerRecord));
            } while (!fitsHdr.Last().StartsWith("END "));
            //Continue through any remaining header padding
            do
            {
                keyindex++;
                bCount = FitsHandleIn.Read(headerRecord, 0, 80);
            } while (!(keyindex % 36 == 0));
            //Get the array dimensions
            int bitpix = Convert.ToInt32(ReadKey("BITPIX"));
            int xAxis = Convert.ToInt32(ReadKey("NAXIS1"));
            int yAxis = Convert.ToInt32(ReadKey("NAXIS2"));
            int totalpixels = xAxis * yAxis;
            //int headerbytes = ImageHeaderLength;
            int totaldata = totalpixels + ImageHeaderLength;

            //Read in the data array in 2880 byte chunks
            byte[] iData = new byte[totalpixels * 2];
            int dataIndex = 0;
            do
            {
                bCount = FitsHandleIn.Read(dataUnit, 0, 2880);
                for (int k = 0; k < bCount; k++)
                {
                    if (dataIndex < iData.Length)
                    {
                        iData[dataIndex] = dataUnit[k];
                        dataIndex++;
                    }
                }
            } while (dataIndex < iData.Length);

            //Close the input file and reopen
            FitsHandleIn.Close();
            FitsHandleIn = File.OpenRead(filepathIn);

            //Create a new file and stream writer
            string filepathOut = filepathIn.Remove(filepathIn.Length - 4);
            filepathOut = filepathOut + ".R.fit";
            FileStream FitsHandleOut = File.OpenWrite(filepathOut);
            //Copy the header record from one fits to the other
            for (int i = 0; i < keyindex; i++)
            {
                bCount = FitsHandleIn.Read(headerRecord, 0, 80);
                FitsHandleOut.Write(headerRecord, 0, 80);
            }
            //Write the data words in reverse order from the last data word to the first
            int idx = iData.Length - 1;
            byte[] dataUnitOut = new byte[2880];
            do
            {
                for (int dc = 0; dc < 2880; dc += 2)
                {
                    if (idx >= 0)
                    {
                        dataUnitOut[dc] = iData[idx - 1];
                        dataUnitOut[dc + 1] = iData[idx];
                        idx = idx - 2;
                    }
                    else
                    {
                        dataUnitOut[dc] = 0;
                        dataUnitOut[dc + 1] = 0;
                        idx = idx - 2;
                    }
                }
                FitsHandleOut.Write(dataUnitOut, 0, 2880);
            } while (idx >= 0);
            FitsHandleOut.Close();
            FitsHandleIn.Close();
            return true;
        }

        private double FitsRAtoRadians(string raStr)
        {
            //Converts the FITS RA string (hours minutes seconds) to radians
            if (raStr == null) return 0;
            string[] raCmp = raStr.Split(' ');
            double hours = Convert.ToDouble(raCmp[0]) + (Convert.ToDouble(raCmp[1]) / 60.0) + (Convert.ToDouble(raCmp[2]) / 3600.0);
            return AstroMath.Transform.HoursToRadians(hours);
        }

        private double FitsKeytoHours(string raStr)
        {
            //Converts the FITS RA string (hours minutes seconds) to radians
            if (raStr == null) return 0;
            string[] raCmp = raStr.Split(' ');
            double hours = Convert.ToDouble(raCmp[0]) + (Convert.ToDouble(raCmp[1]) / 60.0) + (Convert.ToDouble(raCmp[2]) / 3600.0);
            return hours;
        }

        private double FitsHourstoRadians(string decStr)
        {
            //Converts the FITS Dec (degrees minutes seconds) string to radians
            if (decStr == null) return 0;
            string[] decCmp = decStr.Split(' ');
            double degreeDays = Convert.ToDouble(decCmp[0]);
            int sign = Math.Sign(degreeDays);
            double degrees = Math.Abs(Convert.ToDouble(decCmp[0])) + (Convert.ToDouble(decCmp[1]) / 60.0) + (Convert.ToDouble(decCmp[2]) / 3600.0);
            return AstroMath.Transform.DegreesToRadians(sign * degrees);
        }

        private double FitsDegtoDegrees(string decStr)
        {
            //Converts the FITS Dec (degrees minutes seconds) string to radians
            if (decStr == null) return 0;
            string[] decCmp = decStr.Split(' ');
            double degreeDays = Convert.ToDouble(decCmp[0]);
            int sign = Math.Sign(degreeDays);
            double degrees = Math.Abs(Convert.ToDouble(decCmp[0])) + (Convert.ToDouble(decCmp[1]) / 60.0) + (Convert.ToDouble(decCmp[2]) / 3600.0);
            return (sign * degrees);
        }

        private static Image ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        private byte[] ConvertImageToByteArray(Image image, string extension)
        {
            using (var memoryStream = new MemoryStream())
            {
                switch (extension)
                {
                    case ".jpeg":
                    case ".jpg":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".gif":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
                return memoryStream.ToArray();
            }
        }

        private ushort TwosComplementBytesToInteger(ushort highbyte, ushort lowbyte)
        {
            //Data is stored in high byte followed by low byte
            //Create a twos complement integer from high and low bytes
            //  value will range from -32K to + 32K
            Int16 twosComplement = (short)((highbyte << 8) + lowbyte);
            //  add 32K to offset the twos complement
            int val16bits = twosComplement + 0x8000;
            ushort bmVal = (ushort)Math.Min((val16bits / 256), 255);
            return bmVal;
        }

        public Image FitsSubframe(Bitmap fitsBmp, Point center, Size size)
        {
            //Produces a subframed image from a larger image
            //  crops newHeight, newWidth starting at upperCorner
            Point location = new Point(center.X - size.Width / 2, center.Y - size.Height / 2);
            Rectangle framespec = new Rectangle(location, size);
            Bitmap subframe = fitsBmp.Clone(framespec, fitsBmp.PixelFormat);
            return subframe;
        }

        public double RotationToPA(double cd11, double cd12, double cd21, double cd22)
        {
            double pa1;
            //double pa2;
            //double at1 = (Math.Atan2(cd21,cd11));
            //double at2 = (Math.Atan2(-cd12,cd22));
            //double r1 = at1 * (180.0 / Math.PI);
            //double r2 = at2 * (180.0 / Math.PI);
            //double p1 = r1 + 180;
            //double p2 = r2 + 180;
            pa1 = AstroMath.Transform.NormalizeDegreeRange((Math.Atan2(cd21, cd11)) * (180.0 / Math.PI) + 180);
            //pa2 = AstroMath.Transform.NormalizeDegreeRange((Math.Atan2(-cd12, cd22)) * (180.0 / Math.PI) + 180);
            return pa1;
        }

        public Boolean PlateSolvePS2()
        {
            if (FilePath != null)
            {
                Coordinate coords = PlateSolverPS2.StartPlateSolve(FilePath,
                                                                  ObjectRA,
                                                                  ObjectDec,
                                                                  Xaxis * PixelScale,
                                                                  Yaxis * PixelScale,
                                                                  300,
                                                                  @"C:\Program Files (x86)\PlaneWave Instruments\PlateSolve2.28\PlateSolve2.exe");
                if (coords == null)
                    return false;
                else
                {
                    ObjectRA = coords.Ra;
                    ObjectDec = coords.Dec;
                    PA = -coords.PA;
                    PixelScale = coords.PixelScale;
                    CRVAL_RA = Transform.HoursToDegrees(ObjectRA);
                    CRVAL_Dec = ObjectDec;
                    CRPIX_X = Xaxis / 2;
                    CRPIX_Y = Yaxis / 2;
                    double cdelt1 = PixelScale / 3600;
                    double cdelt2 = PixelScale / 3600;  //square pixels assumed
                    //	CD1_1 = CDELT1 * cos(CROTA2)
                    //	CD1_2 = -CDELT2 * sin(CROTA2)
                    //	CD2_1 = CDELT1 * sin(CROTA2)
                    //	CD2_2 = CDELT2 * cos(CROTA2)
                    CD1_1 = -cdelt1 * Math.Cos(AstroMath.Transform.DegreesToRadians(PA));
                    CD1_2 = -cdelt2 * Math.Sin(AstroMath.Transform.DegreesToRadians(PA));
                    CD2_1 = cdelt1 * Math.Sin(AstroMath.Transform.DegreesToRadians(PA));
                    CD2_2 = -cdelt2 * Math.Cos(AstroMath.Transform.DegreesToRadians(PA));
                    return true;
                }
            }
            else return false;
        }


    }
}


