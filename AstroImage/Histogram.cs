using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;


namespace AstroImage
{
    public class Histogram
    {
        private FitsFile ff;

        public Histogram(FitsFile fitsFile)
        {
            ff = fitsFile;
        }


        public (int, int) SpanHistogram(int dropSpan)
        {
            //Determine the span of histogram that covers the all but the dropSpan percent of the range
            int iLowerBound = 0;
            int iUpperBound = UInt16.MaxValue;
            int cumTotal = 0;
            int lowCount = (int)((((double)dropSpan) / 100.0) * (double)(ff.FITS_Vector.Length));
            int highCount = (int)(((double)(1 - ((double)dropSpan / 100.0))) * (double)(ff.FITS_Vector.Length));

            for (int i = 0; i < ff.FITS_Hist.Length; i++)
            {
                if (cumTotal < lowCount) iLowerBound = i;
                else if (cumTotal < highCount) iUpperBound = i;
                else if (cumTotal > highCount) break;
                cumTotal += ff.FITS_Hist[i];
            }
            return (iLowerBound, iUpperBound);
        }

        public (int, int) MedianHistogram()
        {
            //Converts fitsArray to vector for histogram purposes
            //Create an list that can be statistically analyzed
            //Determine the gaussian distribution of the histogram
            double maxBucketValue = ff.FITS_Hist.Max();
            int maxBucketIndex = FindNearestBucketSize(1, ff.FITS_Hist.Length, maxBucketValue);
            int risingBucketIndex = FindNearestBucketSize(1, ff.FITS_Hist.Length, maxBucketValue / 8);
            int descendingBucketIndex = FindNearestBucketSize(maxBucketIndex, ff.FITS_Hist.Length, maxBucketValue / 8);
            int risingBucketValue = (int)BucketLowerBound(0, UInt16.MaxValue, risingBucketIndex);
            int descendingBucketValue = (int)BucketUpperBound(0, UInt16.MaxValue, descendingBucketIndex);
            return (risingBucketValue, descendingBucketValue);
        }

        public Bitmap LinearStretch()
        {
            //Stretches image values based on Max, Min of byte image
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to the average
            //Create Look Up Table for histogram transformation

            Bitmap fitsBMP = new Bitmap(ff.Xaxis, ff.Yaxis);
            int pixVal;
            //(int lowerVal, int upperVal) = MedianHistogram();
            (int lowerVal, int upperVal) = SpanHistogram(1);
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
                //histLUT[i] = 0;

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
            ff.HistUpperBound = upperVal;
            ff.HistLowerBound = lowerVal;

            return fitsBMP;
        }

        public Bitmap LogStretch()
        {
            //Stretches fits image values based on Max, Min of byte image using a Log stretch
            //  This alogrithm subtracts the Min from each element, { multiplies by the
            //     total range divided by the max-min range -- making sure there is not
            //     a divide by zero situation by adding 1 to values
            Bitmap fitsBMP = new Bitmap(ff.Xaxis, ff.Yaxis);
            int pixVal;
            Color newColor;
            (double logMin, double logMax) = MedianHistogram();

            double logAvg = Math.Log(Math.Max((ff.AvgValue - 1), 1));

            for (int iy = 0; iy < ff.Yaxis; iy++)
            {
                for (int ix = 0; ix < ff.Xaxis; ix++)
                {
                    double logX = Math.Log(Math.Max(ff.FITS_Array[ix, iy], (ushort)1));
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

        //public void HistogramEqualization(FitsFile ff)
        //{
        //    //Stretches image values based on Max, Min of byte image
        //    //  This alogrithm subtracts the Min from each element, { multiplies by the
        //    //     total range divided by the max-min range -- making sure there is not
        //    //     a divide by zero situation by adding 1 to the average

        //    int ddata;
        //    double ndata;
        //    //Compute the average of the "middle" 90% of pixels

        //    int datasize = ff.FITS_Vector.Length - ff.ImageHeaderLength;

        //    //Make a probablity mass functiom (PMF) for the histogram, start with it zeroed out
        //    //  and get the number of datapoints
        //    //
        //    int[] pmfdata = new int[256];
        //    for (int i = 0; i < pmfdata.Length; i++)
        //    {
        //        pmfdata[i] = 0;
        //        datasize += 1;
        //    }
        //    for (int j = ff.ImageHeaderLength; j < ff.FITS_Vector.Length; j++)
        //    {
        //        ddata = Convert.ToInt16(ff.FITS_Vector[j]);
        //        pmfdata[ddata] += 1;
        //    }

        //    //Make a cumulative distributive functiom (CDF) for the , start with it zeroed out
        //    //  Normalize to the total number of points
        //    double[] cdfdata = new double[256];
        //    cdfdata[0] = pmfdata[0] / datasize;
        //    for (int k = 1; k < pmfdata.Length; k++)
        //    {
        //        cdfdata[k] = cdfdata[k - 1] + (pmfdata[k] / datasize);
        //    }
        //    //Adjust each value of the image based on it//s

        //    for (int i = ff.ImageHeaderLength; i < ff.FITS_Vector.Length; i++)
        //    {
        //        ddata = Convert.ToInt16(FITS_Vector[i]);
        //        //stretch range from min to max value
        //        // ndata = (255 / MaxValue) * (ddata - MinValue)
        //        ndata = ddata * (cdfdata[ddata]);
        //        //Clip top and bottom of range to 0 and 255 respectively
        //        if (ndata > 255)
        //        {
        //            ndata = 255;
        //        }
        //        if (ndata < 0)
        //        {
        //            ndata = 0;
        //        }
        //        //Convert back into the image array
        //        FITS_Vector[i] = Convert.ToByte(ndata);
        //    }
        //    return;
        //}

        private int FindNearestBucketSize(int startLookingIndex, int stopLookingIndex, double level)
        {
            //find the bucket whose contents is closest to the value level
            int min = int.MaxValue;
            int thisone = 0;
            for (int i = startLookingIndex; i < stopLookingIndex; i++)
            {
                if (Math.Abs(ff.FITS_Hist[i] - level) < min)
                {
                    min = (int)Math.Abs(ff.FITS_Hist[i] - level);
                    thisone = i;
                }
            }
            return thisone;
        }

        private double BucketUpperBound(double histLowest, double histHighest, int bucketIndex)
        {
            //returns the upper bound for the bucket
            return ((histHighest - histLowest) / ff.FITS_Hist.Length) * (bucketIndex + 1);
        }

        private double BucketLowerBound(double histLowest, double histHighest, int bucketIndex)
        {
            //returns the upper bound for the bucket
            return ((histHighest - histLowest) / ff.FITS_Hist.Length) * bucketIndex;
        }





    }
}
