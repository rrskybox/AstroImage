﻿using AstroMath;
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

        private UInt16[,] FITSArray;

        private List<string> fitsHdr = new List<string>();
        private UInt16[] fitsData = new UInt16[1];
        private UInt16[] fitsHist = new UInt16[256];

        const int ImageHeaderLength = 56 + (256 * 4);

        public string FilePath { get; set; }
        public int Xaxis { get; set; }  //Pixels
        public int Yaxis { get; set; }  //Pixels
        public double RA { get; set; }  //OBJCTRA in Hours
        public double Dec { get; set; }  //OBJCTDEC in Degrees
        public double PA { get; set; }  //ORIENTAT in Degrees
        public double XpixSz { get; set; } //XPIXSZ in microns
        public double YpixSz { get; set; }  //YPIXSZ in microns
        public double FocalLength { get; set; }  //FOCALLEN in mm
        public double PixelScale { get; set; }  //arcsec per pixel
        public int MaxValue { get; set; }  //ADU
        public int MinValue { get; set; }  //ADU
        public int AvgValue { get; set; }  //ADU

        public FitsFile(string filepath, bool dataswitch)
        {

            //Opens file set by filepath (assumes it's a FITS formatted file)
            //Reads in header in 80 character strings, while ("END" is found
            //if (dataswitch is true, { an array "FITSimage" is created and populated with the FITS image data as an class Image
            //  otherwise just the header info is retained
            //
            FilePath = filepath;

            int keyindex = -1;
            FileStream FitsHandle = File.OpenRead(filepath);
            do
            {
                keyindex++;
                bCount = FitsHandle.Read(headerRecord, 0, 80);
                //Check for empty file (file error on creation), just opt out if (so
                if (bCount == 0)
                {
                    return;
                }
                fitsHdr.Add(System.Text.Encoding.ASCII.GetString(headerRecord));
            } while (!fitsHdr.Last().StartsWith("END "));
            //Continue through any remaining header padding
            do
            {
                keyindex++;
                bCount = FitsHandle.Read(headerRecord, 0, 80);
            } while (!(keyindex % 36 == 0));

            //Get the array dimensions
            int bitpix = Convert.ToInt32(ReadKey("BITPIX"));
            Xaxis = Convert.ToInt32(ReadKey("NAXIS1"));
            Yaxis = Convert.ToInt32(ReadKey("NAXIS2"));
            RA = FitsKeytoHours(ReadKey("OBJCTRA"));
            Dec = FitsDegtoDegrees(ReadKey("OBJCTDEC"));
            PA = FitsDegtoDegrees(ReadKey("ORIENTAT"));
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
                int integer16;
                int uinteger32;
                Int16 highbyte;
                Int16 lowbyte;

                int bmWidth = Xaxis;
                int bmHeight = Yaxis;
                Array.Resize(ref fitsData, totaldata);

                int bmX = 0;
                int bmY = 0;
                UInt16 bmVal;
                UInt16 bmMin = 255;
                UInt16 bmMax = 0;
                Double bmAvg = 0;

                do
                {
                    bCount = FitsHandle.Read(dataUnit, 0, 2880);
                    for (int k = 0; k <= (bCount - 1); k = k + 2)
                    {
                        if (dataindex < totalpixels)
                        {
                            bmVal = TwosComplementBytesToInteger(dataUnit[k], dataUnit[k + 1]);
                            if (bmVal > bmMax) bmMax = bmVal;
                            if (bmVal < bmMin) bmMin = bmVal;
                            bmAvg += bmVal;

                            fitsData[dataindex] = bmVal;
                            fitsHist[bmVal] += 1;
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

                MaxValue = bmMax;
                MinValue = bmMin;
                AvgValue = (Int16)(bmAvg / totalpixels);
            }
            //Generate Fits image array
            FITSArray = new UInt16[Xaxis, Yaxis];
            for (int iy = 0; iy < Yaxis; iy++)
            {
                for (int ix = 0; ix < Xaxis; ix++)
                {
                    FITSArray[ix, iy] = fitsData[ix + (iy * Xaxis)];
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

        public Bitmap HistogramStretch(int maxVal, int minVal)
        {
            //Stretches image values based on Max, Min of byte image
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to the average
            Bitmap fitsBMP = new Bitmap(Xaxis, Yaxis);
            int pixVal;
            Color newColor;
            //int minVal = MinValue;
            //MaxValue = clipVal;
            int width = Xaxis;
            int height = Yaxis;
            int range = maxVal - minVal + 1;

            int rangeStretch = 255 / range;
            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    pixVal = Math.Min((fitsData[ix + (iy * width)] - minVal) * rangeStretch, 255);
                    newColor = Color.FromArgb(255, pixVal, pixVal, pixVal);
                    fitsBMP.SetPixel(ix, iy, newColor);
                }
            }
            return fitsBMP;
        }

        public Bitmap HistogramLogStretch()
        {
            //Stretches image values based on Max, Min of byte image using a Log stretch
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to values
            Bitmap fitsBMP = new Bitmap(Xaxis, Yaxis);
            int pixVal;
            Color newColor;
            double logMax = Math.Log(Math.Max(MaxValue*.9, 1));
            double logMin = Math.Log(Math.Max(MinValue, 1));
            double logAvg = Math.Log(Math.Max((AvgValue - 1), 1));

            for (int iy = 0; iy < Yaxis; iy++)
            {
                for (int ix = 0; ix < Xaxis; ix++)
                {
                    double logX = Math.Log(Math.Max(FITSArray[ix, iy], (ushort)1));
                    double clipLogX = Math.Max(logX - logAvg, 0);
                    double stretchLogX = clipLogX / logMax;
                    int stretchX = Convert.ToInt32(stretchLogX * 256.0);
                    pixVal = Math.Min(stretchX, 255);
                    newColor = Color.FromArgb(255, pixVal, pixVal, pixVal);
                    fitsBMP.SetPixel(ix, iy, newColor);
                }
            }
            return fitsBMP;
        }

        public void HistogramEqualization()
        {
            //Stretches image values based on Max, Min of byte image
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to the average

            int ddata;
            double ndata;
            //Compute the average of the "middle" 90% of pixels

            int datasize = fitsData.Length - ImageHeaderLength;

            //Make a probablity mass functiom (PMF) for the histogram, start with it zeroed out
            //  and get the number of datapoints
            //
            int[] pmfdata = new int[256];
            for (int i = 0; i < pmfdata.Length; i++)
            {
                pmfdata[i] = 0;
                datasize += 1;
            }
            for (int j = ImageHeaderLength; j < fitsData.Length; j++)
            {
                ddata = Convert.ToInt16(fitsData[j]);
                pmfdata[ddata] += 1;
            }

            //Make a cumulative distributive functiom (CDF) for the , start with it zeroed out
            //  Normalize to the total number of points
            double[] cdfdata = new double[256];
            cdfdata[0] = pmfdata[0] / datasize;
            for (int k = 1; k < pmfdata.Length; k++)
            {
                cdfdata[k] = cdfdata[k - 1] + (pmfdata[k] / datasize);
            }
            //Adjust each value of the image based on it//s

            for (int i = ImageHeaderLength; i < fitsData.Length; i++)
            {
                ddata = Convert.ToInt16(fitsData[i]);
                //stretch range from min to max value
                // ndata = (255 / MaxValue) * (ddata - MinValue)
                ndata = ddata * (cdfdata[ddata]);
                //Clip top and bottom of range to 0 and 255 respectively
                if (ndata > 255)
                {
                    ndata = 255;
                }
                if (ndata < 0)
                {
                    ndata = 0;
                }
                //Convert back into the image array
                fitsData[i] = Convert.ToByte(ndata);
            }
            return;
        }

        public Point RADECtoImageXY(double hoursRA, double degreesDec)
        {
            //RA in hours, Dec in degrees
            //Return x,y point in image
            //Get the image size and resolution and center of image in X/Y in radians
            double radiansRA = Transform.HoursToRadians(Convert.ToDouble(hoursRA));
            double radiansDec = Transform.DegreesToRadians(Convert.ToDouble(degreesDec));

            double height = Yaxis;
            double yCen = height / 2.0;
            double width = Xaxis;
            double xCen = width / 2.0;
            //double hRes = AstroMath.Transform.DegreesToRadians(Ypixsz / 3600.0);  //Radians per pixel
            double xResArcSec = ((206.265 / FocalLength) * XpixSz);
            double hRes = AstroMath.Transform.DegreesToRadians(xResArcSec / 3600.0);  //Radians per pixel
            //double wRes = AstroMath.Transform.DegreesToRadians(XpixSz / 3600.0);  //Readians per pixel
            double yResArcSec = (206.265 / FocalLength) * YpixSz;
            double wRes = AstroMath.Transform.DegreesToRadians(yResArcSec / 3600.0);  //Readians per pixel

            //Get RA/Dec for the center of the image 
            double centerRA = Transform.HoursToRadians(RA);
            double centerDec = Transform.DegreesToRadians(Dec);
            double iPA = Transform.DegreesToRadians(PA);
            //compute unrotated RA/Dec for +RA = -x; +Dec = y
            //delta_ra = ra - ra0;          /* determine difference in RA */
            double deltaRA = radiansRA - centerRA;
            //x1 = cos(dec) * sin(delta_ra);
            double urX = Math.Cos(radiansDec) * Math.Sin(deltaRA);
            //y1 = sin(dec) * cos(dec0) - cos(dec) * cos(delta_ra) * sin(dec0);
            double urY = (Math.Sin(radiansDec) * Math.Cos(centerDec)) - (Math.Cos(radiansDec) * Math.Cos(deltaRA) * Math.Sin(centerDec));
            double urXas = (-1.0 * urX) / wRes;
            double urYas = urY / hRes;
            //rotate based on PA
            Point newPt = new Point((int)urXas, (int)urYas);
            Point rotPt = AstroMath.Polar2D.XYRotation(newPt, iPA);
            //Point rotPt = new Point (0, 0);
            //offset from upper left corner (0,0)
            Point offPt = new Point((int)xCen + rotPt.X, (int)yCen - rotPt.Y);
            return offPt;
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

        public Boolean MakePlate()
        {
            Coordinate coords = PlateSolver.StartPlateSolve(FilePath,
                                                              RA,
                                                              Dec,
                                                              Xaxis * PixelScale,
                                                              Yaxis * PixelScale,
                                                              300,
                                                              @"C:\Program Files (x86)\PlaneWave Instruments\PlateSolve2.28\PlateSolve2.exe");
            if (coords == null)
            {
                return false;
            }
            else
            {
                RA = coords.Ra;
                Dec = coords.Dec;
                PA = -coords.PA;
                PixelScale = coords.PixelScale;
                return true;
            }
        }


    }
}


