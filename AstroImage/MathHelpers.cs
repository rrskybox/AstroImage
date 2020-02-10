using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroImage
{
    public static class MathHelpers
    {
        public static double DegToRad(double deg)
        {
            double rad = deg * Math.PI / 180;

            return rad;
        }

        public static double RadToDeg(double rad)
        {
            double deg = 180 * rad / Math.PI;

            return deg;
        }

        public static double HoursToRad(double hours)
        {
            double rad = hours * Math.PI / 12;

            return rad;
        }

        public static double RadToHours(double rad)
        {
            double hours = 12 * rad / Math.PI;

            return hours;
        }
    }
}
