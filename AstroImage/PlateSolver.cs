using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AstroMath;

// PlateSolve2.exe 
//  (Right ascension in radians),
//  (Declination in radians),
//  (x dimension in radians),
//  (y dimension in radians),
//  (Number of regions to search),
//  (fits filename),
//  (wait time at the end)

namespace AstroImage
{
    public class Coordinate
    {
        public double Ra { get; set; }
        public double Dec { get; set; }
        public double PA { get; set; }
        public double PixelScale { get; set; }
    }

    public static class PlateSolver
    {
        private static string fitsImageFilename;
        public static Coordinate StartPlateSolve(string fileName, double raHrs, double decDegrees, double fieldWidthArcSec, double fieldHeightArcSec, int maxTiles, string solverPath)
        {
            //Command line:
            //PlateSolve2.exe(Right ascension in radians),
            //               (Declination in radians),
            //               (x dimension in radians),
            //               (y dimension in radians),
            //               (Number of regions to search),
            //               (fits filename),
            //               (wait time at the end) 

            Coordinate coordinate = null;

            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = solverPath;
            proc.StartInfo.Arguments =
                Transform.HoursToRadians(raHrs).ToString("0.00000", CultureInfo.InvariantCulture) + "," +
                Transform.DegreesToRadians(decDegrees).ToString("0.00000", CultureInfo.InvariantCulture) + "," +
                Transform.DegreesToRadians(fieldWidthArcSec / 3600.0).ToString("0.000", CultureInfo.InvariantCulture) + "," +
                Transform.DegreesToRadians(fieldHeightArcSec / 3600.0).ToString("0.000", CultureInfo.InvariantCulture) + "," +
                maxTiles.ToString() + "," +
                fileName + "," +
                "0";
            fitsImageFilename = fileName;
            proc.Start();
            while (!proc.HasExited) { Thread.Sleep(1000); }

            string apmFileName = fileName.Replace(".fit", ".apm");

            //string apmFileName = Path.Combine(Path.GetDirectoryName(fitsImageFilename),
            //                                  Path.ChangeExtension(Path.GetFileNameWithoutExtension(fitsImageFilename), "apm"));
            coordinate = ReadApmFile(apmFileName);

            return coordinate;
        }



        private static Coordinate ReadApmFile(string fileName)
        {
            //.APM Structure:
            //
            // RA(radians),Dec(radians),?
            // PixScale(ArcSec/Pixel),PA(degrees),?,?,?
            // "Valid Plate Solution"
            Coordinate result = null;

            try
            {
                var lines = File.ReadAllLines(fileName);
                if (lines.Count() >= 3 && lines[2].StartsWith("Valid"))
                {
                    string[] firstLine = HandleSeparators(lines[0]).Split(',');
                    string[] secondLine = HandleSeparators(lines[1]).Split(',');
                    result = new Coordinate
                    {
                        Ra = Transform.RadiansToHours(double.Parse(firstLine[0], CultureInfo.InvariantCulture)),
                        Dec = Transform.RadiansToDegrees(double.Parse(firstLine[1], CultureInfo.InvariantCulture)),
                        PixelScale = double.Parse(secondLine[0], CultureInfo.InvariantCulture),
                        PA = double.Parse(secondLine[1], CultureInfo.InvariantCulture),
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }



        private static string HandleSeparators(string input)
        {
            //Handle case when system decimal separator is , and output looks like this:
            //0,88122,359,13,-1,00013,-0,00017,404
            string result = null;
            if (!input.Contains("."))
            {
                var split = input.Split(',');
                for (int i = 0; i < split.Count(); i++)
                {
                    if (i == split.Count() - 1)
                    {
                        result += split[i];
                    }
                    else if (i % 2 == 0)
                    {
                        result += split[i] + ".";
                    }
                    else
                    {
                        result += split[i] + ",";
                    }
                }
            }
            else
            {
                result = input;
            }

            return result;
        }
    }


}
