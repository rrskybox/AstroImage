using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroImage
{
    public partial class Hough
    {

        public static (double intercept,double slope) HoughTransform(List<double> xP, List<double> yP, int thetaCount, int rangeCount)
        {
            //Runs Hough transform on list of diagram points
            //
            const double halfPI = Math.PI / 2;

            //The increment on votes will be 2 pi /votes = accumlator theta size
            double thetaIncrement = 2 * Math.PI / thetaCount;
            double thetaMin = -Math.PI;
            //The maximum range (r) can be no greater than largest abs(x) + abs(y) value, e.g. r = x cos theta + y sin theta
            double rangeMax = 0;
            for (int i = 0; i < xP.Count; i++)
            {
                double r = Math.Abs(xP[i]) + Math.Abs(yP[i]);
                if (r > rangeMax)
                    rangeMax = r;
            }
            double rangeMin = -rangeMax; //may chnage this later
            //Calculate the range value for each incremental index
            double rangeIncrement =  rangeMax-rangeMin / rangeCount;
            //Set accumlator range to +/- maxRange
            //Create accumulator array size 
            int[,] accumulator = new int[rangeCount, thetaCount];
            //Convert diagramXY to NormalPoint array
            for (int p = 0; p < xP.Count; p++)
            {
                for (int t = 0; t < thetaCount; t++)
                {
                    double theta = halfPI - (t * thetaIncrement);
                    double rangePoint = xP[p] * Math.Sin(theta) + yP[p] * Math.Cos(theta);
                    //the range runs from - rangeMax to + rangeMax
                    //  the index will be 2 * range/max 
                    int rangeBucket = Convert.ToInt32((rangePoint-rangeMin)/rangeIncrement);
                    //Add vote to range/theta
                    accumulator[rangeBucket, t]++;
                }
            }
            //Find max voted in normal space
            int maxVote = 0;
            int votedRangeIndex = 0;
            int votedThetaIndex = 0;
            for (int r = 0; r < rangeCount; r++)
                for (int t = 0; t < thetaCount; t++)
                {
                    int vote = accumulator[r, t];
                    if ( vote > maxVote)
                    {
                        votedRangeIndex = r;
                        votedThetaIndex = t;
                        maxVote = vote;
                    }
                }
            double votedRange = votedRangeIndex*rangeIncrement+rangeMin;
            double votedTheta = votedThetaIndex*thetaIncrement+thetaMin;
            return (votedRange,votedTheta);
        }
    }
}
