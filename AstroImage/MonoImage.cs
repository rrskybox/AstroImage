using System;
using System.Drawing;
using System.Runtime.InteropServices;

using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroImage
{
    public class MonoImage
    {
        //Produces a grayscale bitmap from a FITS integer array scaled to values from 0 - 255
        public static Bitmap MakeMonochromeBitmap(UInt16[,] fitsdata, int maxAdu)
        {
            int IMAGE_WIDTH = fitsdata.GetLength(0);
            int IMAGE_HEIGHT = fitsdata.GetLength(1);
            var b16bpp = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            var rect = new Rectangle(0, 0, IMAGE_WIDTH, IMAGE_HEIGHT);
            var bitmapData = b16bpp.LockBits(rect, ImageLockMode.WriteOnly, b16bpp.PixelFormat);
            //var numberOfBytes = bitmapData.Stride * IMAGE_HEIGHT;
            var bitmapBytes = new byte[IMAGE_WIDTH * IMAGE_HEIGHT * 2];

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    int i = y * IMAGE_WIDTH * 2 + x * 2;
                    ushort l = ScaleToUshort(fitsdata[x, y], maxAdu);
                    ushort[] source = new ushort[] { l };
                    byte[] target = new byte[source.Length * sizeof(ushort)];
                    Buffer.BlockCopy(source, 0, target, 0, source.Length * sizeof(ushort));
                    bitmapBytes[i] = target[0];
                    bitmapBytes[i + 1] = target[1];
                }
            }
            var ptr = bitmapData.Scan0;
            Marshal.Copy(bitmapBytes, 0, ptr, bitmapBytes.Length);
            b16bpp.UnlockBits(bitmapData);
            return b16bpp;
        }

        private static ushort ScaleToUshort(int intData, int maxVal)
        {
            //Scales positive integer data to unsigned short data
            UInt16 a = Convert.ToUInt16((intData) >> 8);

            return (ushort)(Math.Min(intData, maxVal));
        }


    }
}
