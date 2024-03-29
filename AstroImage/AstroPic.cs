﻿using AstroMath;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AstroImage
{ 
    public partial class AstroPic
    {
        public Bitmap PixImage { get; set; }
        private FitsFile fitsIn;

        public AstroPic(ref FitsFile af)
        {
            //Plate solve image and update centerRA, centerDec, centerPA
            fitsIn = af;
            if ((fitsIn.PixelScale == 0) && (fitsIn.FocalLength != 0))
                fitsIn.PixelScale = (206.265 / fitsIn.FocalLength) * fitsIn.XpixSz;
            return;
        }

        public void LinearStretch()
        {
            Histogram hi = new Histogram(ref fitsIn, Histogram.StretchType.Linear);
            PixImage = hi.NormalStretch(ref fitsIn);
            return;
        }

        public void LogStretch(ref FitsFile fitsIn)
        {
            Histogram hi = new Histogram(ref fitsIn, Histogram.StretchType.Log);
            PixImage = hi.NormalStretch(ref fitsIn);
            return;
        }

        public void ArcSinhStretch()
        {
            Histogram hi = new Histogram(ref fitsIn, Histogram.StretchType.Asinh);
            PixImage = hi.NormalStretch(ref fitsIn);
            return;
        }

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public Bitmap AddCrossHair(Point position, int symSize, int lineWidth)
        {
            //Creates Yellow + with open center at location x,y in bitmap
            //  of size 1/10 of the height, with an internal opening of 
            //  1/10 the line length
            Color clr = Color.HotPink;
            Bitmap bm = (Bitmap)PixImage;
            //If null image for some reason, then just return
            if (bm == null)
                return null;

            int maxPixX = bm.Width;
            int maxPixY = bm.Height;
            if (position.X > maxPixX)
                position.X = maxPixX - 1;
            if (position.Y > maxPixY)
                position.Y = maxPixY - 1;

            int holeLen = (int)(symSize * .2);
            int xCen = position.X;
            int yCen = position.Y;
            int xMin = xCen - symSize / 2;
            int xMax = xCen + symSize / 2;
            int yMin = yCen - symSize / 2;
            int yMax = yCen + symSize / 2;
            if (xMin < 0) xMin = 0;
            if (xMax < 0) xMax = 0;
            if (xMax > bm.Width) xMax = bm.Width;
            if (yMin < 0) yMin = 0;
            if (yMax < 0) yMax = 0;
            if (yMax > bm.Height) yMax = bm.Height;

            //make left line, right line, top line, bottom line
            for (int s = -lineWidth / 2; s < +lineWidth / 2; s++)
            {
                for (int i = xMin; i < (xCen - holeLen); i++)
                { if (yCen + s >= 0 && yCen + s < maxPixY) bm.SetPixel(i, yCen + s, clr); }
                for (int i = xCen + holeLen; i < xMax; i++)
                { if (yCen + s >= 0 && yCen + s < maxPixY) bm.SetPixel(i, yCen + s, clr); }
                for (int i = yMin; i < (yCen - holeLen); i++)
                { if (xCen + s >= 0 && xCen + s < maxPixX) bm.SetPixel(xCen + s, i, clr); }
                for (int i = yCen + holeLen; i < yMax; i++)
                { if (xCen + s >= 0 && xCen + s < maxPixX) bm.SetPixel(xCen + s, i, clr); }
            }
            return bm;
        }


        public Image Zoom(int zoom)
        {
            Bitmap bmp = new Bitmap(PixImage, PixImage.Size);
            Graphics g = Graphics.FromImage(bmp);
            Size sizeUp = new Size(bmp.Size.Width / zoom, bmp.Size.Height / zoom);
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
            double rot = Transform.DegreesToRadians(rotation);
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
            //x// = x cos deltaPA - y sin deltaPA
            // double angleR = Math.PI * angleD / 180.0;
            // angleR = Math.PI * -90.001 / 180.0;
            double dX = (X * Math.Cos(angleR)) - (Y * Math.Sin(angleR));
            return (Convert.ToInt32(dX));
        }

        private int TransformY(double X, double Y, double angleR)
        {
            //Computes Y coordinate of a rotation on X,Y through a rotation of angle degrees
            //X in pixels, Y in pixels, angle in radians
            //y// = y cos deltaPA + x sin deltaPA
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

        public Image ResizeImage(Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = PixImage.Width;
                int originalHeight = PixImage.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            Image newImage = new Bitmap(newWidth, newHeight);
            // C#
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(PixImage, 0, 0, newWidth, newHeight);
            }
            return newImage;

        }
    }
}
