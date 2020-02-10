using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AstroImage
{
    public partial class AstroPic
    {
        public Bitmap PixImage { get; set; }
        private FitsFile fitsIn;

        public AstroPic(FitsFile af)
        {
            //Plate solve image and update centerRA, centerDec, centerPA
            fitsIn = af;
            if ((fitsIn.PixelScale == 0) && (fitsIn.FocalLength != 0))
                fitsIn.PixelScale = (206.265 / fitsIn.FocalLength) * fitsIn.XpixSz;
            if (af.MakePlate()) PixImage = fitsIn.HistogramLogStretch();
            else PixImage = null;
            return;
        }

        public Bitmap AddCrossHair(Point position, int symSize, int lineWidth)
        {
            //Creates Yellow + with open center at location x,y in bitmap
            //  of size 1/10 of the height, with an internal opening of 
            //  1/10 the line length
            Bitmap bm = (Bitmap)PixImage;
            int holeLen = (int)(symSize * .2);
            int x = position.X;
            int y = position.Y;
            if (x < lineWidth) x = lineWidth;
            if (y < lineWidth) y = lineWidth;
            int xMin = x - symSize / 2;
            int xMax = x + symSize / 2;
            int yMin = y - symSize / 2;
            int yMax = y + symSize / 2;
            if (xMin < 0) xMin = 0;
            if (xMax < 0) xMax = 0;
            if (xMax > bm.Width) xMax = bm.Width;
            if (yMin < 0) yMin = 0;
            if (yMax < 0) yMax = 0;
            if (yMax > bm.Height) yMax = bm.Height;

            //make left line, right line, top line, bottom line
            for (int s = -lineWidth / 2; s < +lineWidth / 2; s++)
            {
                for (int i = xMin; i < (x - holeLen); i++)
                { bm.SetPixel(i, y + s, Color.Yellow); }
                for (int i = x + holeLen; i < xMax; i++)
                { bm.SetPixel(i, y + s, Color.Yellow); }
                for (int i = yMin; i < (y - holeLen); i++)
                { bm.SetPixel(x + s, i, Color.Yellow); }
                for (int i = y + holeLen; i < yMax; i++)
                { bm.SetPixel(x + s, i, Color.Yellow); }
            }
            return bm;
        }

        public static Image Zoom(Image img, Size size)
        {
            Bitmap bmp = new Bitmap(img, size);
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            return bmp;
        }

        public Image RotateTranslateCrop(Point center, double rotation, Size size)
        {
            //Rotates the fits array and crops out a centered rectangle
            //
            //Rotation:  R(x) = x = Xcosθ – Ysinθ; R(y) = y = Xsinθ + Ycosθ
            //First time through: create subframed and translated current and reference images registered to the galaxy center
            // get the average pixel value for the current and reference images and compute relative intensity
            //  between the two images. 
            Bitmap scurImage = new Bitmap(size.Width, size.Height);
            Color bigImagePixel;
            double rot = MathHelpers.DegToRad(rotation);
            for (int iXp = 0; iXp < size.Width; iXp++)
            {
                for (int iYp = 0; iYp < size.Height; iYp++)
                {
                    int iXd = iXp - size.Width / 2;
                    int iYd = iYp - size.Height / 2;
                    int iXr = center.X + TransformX(iXd, -iYd, rot);
                    int iYr = center.Y - TransformY(iXd, -iYd, rot);
                    //Subtract adjusted reference image pixel intensity from current image pixel intensity
                    //  and put it in the subframe array
                    // int curPix = curImage.GetPixel(iXc, iYc);
                    //int curPix = FITSArray[iXr, iYr];
                    //Color newColor = Color.FromArgb(255, curPix, curPix, curPix);
                    if (IsInside(0, PixImage.Width - 1, 0, PixImage.Height - 1, iXr, iYr))
                        bigImagePixel = PixImage.GetPixel(iXr, iYr);
                    else bigImagePixel = Color.Black;
                    scurImage.SetPixel(iXp, iYp, bigImagePixel);
                }
            }
            return scurImage;
        }

        private int TransformX(double X, double Y, double angleR)
        {
            //Computes X coordinate of a rotation on X,Y through a rotation of angle degrees
            //X in pixels, Y in pixels, angle in radians
            //x' = x cos deltaPA - y sin deltaPA
            // double angleR = Math.PI * angleD / 180.0;
            // angleR = Math.PI * -90.001 / 180.0;
            double dX = (X * Math.Cos(angleR)) - (Y * Math.Sin(angleR));
            return (Convert.ToInt32(dX));
        }

        private int TransformY(double X, double Y, double angleR)
        {
            //Computes Y coordinate of a rotation on X,Y through a rotation of angle degrees
            //X in pixels, Y in pixels, angle in radians
            //y' = y cos deltaPA + x sin deltaPA
            //double angleR = Math.PI * angleD / 180.0;
            //angleR = Math.PI * -90.001 / 180.0;
            double dY = (Y * Math.Cos(angleR)) + (X * Math.Sin(angleR));
            return (Convert.ToInt32(dY));
        }

        private bool IsInside(int minX, int maxX, int minY, int maxY, int valX, int valY)
        {
            if (((valX >= minX) && (valX <= maxX)) && ((valY >= minY) && (valY <= maxY)))
                return true;
            else return false;
        }
    }
}
