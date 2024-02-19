using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics.Tracing;
using MathNet.Numerics;


namespace AstroImage
{
    public class Histogram
    {
        //private FitsFile ff;

        const int HistBuckets = ushort.MaxValue;

        public int[] FITS_Hist;
        public double FITS_Hist_BucketWidth { get; set; }
        public int HistWhiteClipIndex { get; set; }
        public int HistBlackClipIndex { get; set; }
        public int HistPeakIndex { get; set; }

        public enum StretchType
        {
            None,
            Linear,
            Log,
            Asinh
        }

        public Histogram(ref FitsFile ffBase, StretchType st)
        {
            FITS_Hist_BucketWidth = ushort.MaxValue / HistBuckets;
            FITS_Hist = new int[HistBuckets];
            switch (st)
            {
                case StretchType.None:
                    FillHistogram(ref ffBase);
                    break;
                case StretchType.Linear:
                    FillHistogram(ref ffBase);
                    break;
                case StretchType.Log:
                    FillHistogram(ref ffBase);
                    break;
                case StretchType.Asinh:
                    FillHistogramAsinh(ref ffBase);
                    break;
                default:
                    FillHistogram(ref ffBase);
                    break;
            }
        }

        private void FillHistogram(ref FitsFile ff)
        {
            //Convert vector to histogram, do not modify fits data
            for (int i = 0; i < ff.FITS_Vector.Length; i++)
                FITS_Hist[(int)(ff.FITS_Vector[i] / FITS_Hist_BucketWidth)] += 1;
        }

        public (int, int) SpanHistogram(ref FitsFile ff, int dropSpan)
        {
            //Determine the span of histogram that covers the all but the dropSpan percent of the range
            int iLowerBound = 0;
            int iUpperBound = UInt16.MaxValue;
            int cumTotal = 0;
            int lowCount = (int)((((double)dropSpan) / 100.0) * (double)(ff.FITS_Vector.Length));
            int highCount = (int)(((double)(1 - ((double)dropSpan / 100.0))) * (double)(ff.FITS_Vector.Length));

            for (int i = 0; i < FITS_Hist.Length; i++)
            {
                if (cumTotal < lowCount) iLowerBound = i;
                else if (cumTotal < highCount) iUpperBound = i;
                else if (cumTotal > highCount) break;
                cumTotal += FITS_Hist[i];
            }
            return (iLowerBound, iUpperBound);
        }

        public void MedianHistogram()
        {
            //Converts fitsArray to vector for histogram purposes
            //Create an list that can be statistically analyzed
            //Determine the gaussian distribution of the histogram
            HistPeakIndex = FindFullestBucketIndex();
            HistBlackClipIndex = FindNearestBucketSize(1, HistPeakIndex, (int)(FITS_Hist[HistPeakIndex] * 0.8));
            HistWhiteClipIndex = FindNearestBucketSize(HistPeakIndex, FITS_Hist.Length, (Int16)(FITS_Hist[HistPeakIndex] * 0.125));
            return;
        }

        //Common stretch for normalization after a histogram stretch
        public Bitmap NormalStretch(ref FitsFile ff)
        {
            //Stretches image values based on Max, Min of byte image
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to the average
            //Create Look Up Table for histogram transformation

            Bitmap fitsBMP = new Bitmap(ff.Xaxis, ff.Yaxis);
            MedianHistogram(); //set upper and lower bounds (max/min)
            double range = HistWhiteClipIndex - HistBlackClipIndex;
            //double stretch = UInt16.MaxValue / range;
            double stretch = 255.0 / range;

            for (int iy = 0; iy < ff.Yaxis; iy++)
                for (int ix = 0; ix < ff.Xaxis; ix++)
                {
                    double transVal = ((ff.FITS_Array[ix, iy] - HistBlackClipIndex) * stretch);
                    //int pixVal = (int)(transVal / 256) - 1;
                    int pixVal = (int)transVal;
                    //clip lower to black
                    if (pixVal < 0)
                        pixVal = 0;
                    //clip upper to white
                    else if (pixVal > 255)
                        pixVal = 255;
                    fitsBMP.SetPixel(ix, iy, Color.FromArgb(255, pixVal, pixVal, pixVal));
                }
            return fitsBMP;
        }

        public Bitmap LinearStretch(ref FitsFile ff)
        {
            //Stretches image values based on Max, Min of byte image
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to the average
            //Create Look Up Table for histogram transformation

            Bitmap fitsBMP = new Bitmap(ff.Xaxis, ff.Yaxis);
            int pixVal;
            //(int lowerVal, int upperVal) = MedianHistogram();
            (int lowerVal, int upperVal) = SpanHistogram(ref ff, 1);
            int domain = UInt16.MaxValue;
            int range = upperVal - lowerVal + 1;
            double increment = (double)domain / (double)range;

            UInt16[] histLUT = new UInt16[domain];
            for (int i = 0; i < lowerVal; i++)
                histLUT[i] = 0;
            for (int i = lowerVal; i < upperVal; i++)
                histLUT[i] = (UInt16)((i - lowerVal) * increment);
            for (int i = upperVal; i < domain; i++)
                histLUT[i] = UInt16.MaxValue;

            for (int iy = 0; iy < ff.Yaxis; iy++)
            {
                for (int ix = 0; ix < ff.Xaxis; ix++)
                {
                    UInt16 transVal = histLUT[ff.FITS_Array[ix, iy]];
                    pixVal = (transVal / 256) - 1;
                    if (pixVal < 0)
                        pixVal = 0;
                    fitsBMP.SetPixel(ix, iy, Color.FromArgb(255, pixVal, pixVal, pixVal));
                }
            }
            //ff.HistUpperBound = upperVal;
            //ff.HistLowerBound = lowerVal;

            return fitsBMP;
        }

        public Bitmap LogStretch(ref FitsFile ff)
        {
            //Stretches fits image values based on Max, Min of byte image using a Log stretch
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to values
            Bitmap fitsBMP = new Bitmap(ff.Xaxis, ff.Yaxis);
            int pixVal;
            Color newColor;
            MedianHistogram();

            double logAvg = Math.Log(Math.Max((ff.AvgValue - 1), 1));

            for (int iy = 0; iy < ff.Yaxis; iy++)
            {
                for (int ix = 0; ix < ff.Xaxis; ix++)
                {
                    double logX = Math.Log(Math.Max(ff.FITS_Array[ix, iy], (ushort)1));
                    double clipLogX = Math.Max(logX - logAvg, 0);
                    double stretchLogX = clipLogX / HistWhiteClipIndex;
                    int stretchX = Convert.ToInt32(stretchLogX * 256.0);
                    pixVal = Math.Min(stretchX, 255);
                    newColor = Color.FromArgb(255, pixVal, pixVal, pixVal);
                    fitsBMP.SetPixel(ix, iy, newColor);
                }
            }
            return fitsBMP;
        }

        public void FillHistogramAsinh(ref FitsFile ff, double a = 0.1)
        {
            //        An asinh stretch.
            //The stretch is given by:
            //..math::
            //    y = \frac{ {\rm asinh} (x / a)}/ { {\rm asinh} (1 / a)}.
            //Parameters
            //----------
            //a: float, optional
            //    The ``a`` parameter used in the above formula.The value of this
            //    parameter is where the asinh curve transitions from linear to
            //    logarithmic behavior, expressed as a fraction of the normalized
            //    image.The stretch becomes more linear as the ``a`` value is
            //    increased. ``a`` must be greater than 0.Default is 0.1.
            //Sinh-1 x = ln(x + √[1+x^2])

            //Convert fits array to asinh'd array and fill in histogram accordingly
            for (int iy = 0; iy < ff.Yaxis; iy++)
            {
                for (int ix = 0; ix < ff.Xaxis; ix++)
                {
                    //var vPix = ff.FITS_Array[ix, iy];
                    //var aV = Asinh(vPix);
                    //var bV = aV / (2 * Math.PI);
                    //var cV = bV * UInt16.MaxValue;
                    //var dV = Math.Log(vPix + Math.Sqrt(1 + (vPix * vPix)));
                    ushort pix = (ushort)((Asinh(ff.FITS_Array[ix, iy]) / 11.78348681) * UInt16.MaxValue);
                    ff.FITS_Array[ix, iy] = pix;
                    FITS_Hist[(int)(pix / FITS_Hist_BucketWidth)] += 1;
                }
            }
        }

        private double Asinh(double x) => Math.Log(x + Math.Sqrt(1 + (x * x)));

        private int FindNearestBucketSize(int startLookingIndex, int stopLookingIndex, int count)
        {
            //find the bucket whose contents is closest to the value level
            int diff = Int16.MaxValue;
            int thisone = startLookingIndex;
            for (int i = startLookingIndex; i < stopLookingIndex; i++)
            {
                if (Math.Abs(FITS_Hist[i] - count) < diff)
                {
                    diff = (int)Math.Abs(FITS_Hist[i] - count);
                    thisone = i;
                }
            }
            return thisone;
        }

        private int FindFullestBucketIndex()
        {
            //find the bucket whose contents is closest to the value level
            int largestSize = 0;
            int noThisOne = 0;
            for (int i = 0; i < FITS_Hist.Length; i++)
            {
                if (FITS_Hist[i] > largestSize)
                {
                    largestSize = FITS_Hist[i];
                    noThisOne = i;
                }
            }
            return noThisOne;
        }

        private double BucketUpperBound(double histLowest, double histHighest, int bucketIndex)
        {
            //returns the upper bound for the bucket
            return ((histHighest - histLowest) / FITS_Hist.Length) * (bucketIndex + 1);
        }

        private double BucketLowerBound(double histLowest, double histHighest, int bucketIndex)
        {
            //returns the upper bound for the bucket
            return ((histHighest - histLowest) / FITS_Hist.Length) * bucketIndex;
        }
    }
}
