using System.Drawing;


namespace AstroImage
{
    public class CImage
    {
        public byte[] Grid;
        public int width, height, N_Bits, nLoop, denomProg;
        Color[] Palette;

        public CImage(int nx, int ny, int nbits) // constructor
        {
            width = nx;
            height = ny;
            N_Bits = nbits;
            Palette = new Color[256];
            Grid = new byte[width * height * (N_Bits / 8)];
        }

        public CImage(int nx, int ny, int nbits, byte[] img) // constructor
        {
            width = nx;
            height = ny;
            N_Bits = nbits;
            Palette = new Color[256];

            Grid = new byte[width * height * (N_Bits / 8)];
            for (int i = 0; i < width * height * N_Bits / 8; i++) Grid[i] = img[i];
        }



    }
}
