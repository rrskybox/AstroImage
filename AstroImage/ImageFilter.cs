using System;

namespace AstroImage
{
    public class ImageFilter
    {
        public static int SigmaFastFilter(FitsFile ffData, int hWind, int Toleranz)
        {
            // Sigma filter with doubled calculation of the output values for gray value images.
            int gv, y1, yEnd, yStart;
            //Inp.Grid = new byte[Inp.width * Inp.height * Inp.N_Bits / 8];
            int[] hist = new int[256];
            for (int y = 0; y < ffData.Yaxis; y++) // =======================================
            {
                yStart = Math.Max(y - hWind, 0);
                yEnd = Math.Min(y + hWind, ffData.Yaxis - 1);
                for (int x = 0; x < ffData.Xaxis; x++) //====================================
                {
                    if (x == 0) //-----------------------------------------------
                    {
                        for (gv = 0; gv < 256; gv++) hist[gv] = 0;
                        for (y1 = yStart; y1 <= yEnd; y1++)
                            for (int xx = 0; xx <= hWind; xx++) hist[ffData.FITS_Vector[xx + y1 * ffData.Xaxis]]++;
                    }
                    else
                    {
                        int x1 = x + hWind, x2 = x - hWind - 1;
                        if (x1 < ffData.Xaxis)
                            for (y1 = yStart; y1 <= yEnd; y1++) hist[ffData.FITS_Vector[x1 + y1 * ffData.Xaxis]]++;
                        if (x2 >= 0)
                            for (y1 = yStart; y1 <= yEnd; y1++)
                            {
                                hist[ffData.FITS_Vector[x2 + y1 * ffData.Xaxis]]--;
                                if (hist[ffData.FITS_Vector[x2 + y1 * ffData.Xaxis]] < 0) return -1;
                            }
                    } //---------------- end if (x==0) ------------------------
                    int Sum = 0;
                    int nPixel = 0;
                    int prov;
                    int gvMin = Math.Max(0, ffData.FITS_Vector[x + ffData.Xaxis * y] - Toleranz),
                        gvMax = Math.Min(255, ffData.FITS_Vector[x + ffData.Xaxis * y] + Toleranz);
                    for (gv = gvMin; gv <= gvMax; gv++)
                    {
                        Sum += gv * hist[gv]; nPixel += hist[gv];
                    }
                    if (nPixel > 0) prov = (Sum + nPixel / 2) / nPixel;
                    else prov = ffData.FITS_Vector[x + ffData.Xaxis * y];
                    Sum = nPixel = 0;
                    gvMin = Math.Max(0, prov - Toleranz);
                    gvMax = Math.Min(255, prov + Toleranz);
                    for (gv = gvMin; gv <= gvMax; gv++)
                    {
                        Sum += gv * hist[gv]; nPixel += hist[gv];
                    }
                    if (nPixel > 0) ffData.FITS_Vector[x + ffData.Xaxis * y] = (byte)((Sum + nPixel / 2) / nPixel);
                    else ffData.FITS_Vector[x + ffData.Xaxis * y] = ffData.FITS_Vector[x + ffData.Xaxis * y];
                } //================== end for (int x... ======================
            } //==================== end for (int y... ========================
            return 1;
            //********************** end SigmaFastFilter **********************************
        }

        public int SigmaFastFilter(CImage Inp, int hWind, int Toleranz)
        {
            // Sigma filter with doubled calculation of the output values for gray value images.
            int gv, y1, yEnd, yStart;
            Inp.Grid = new byte[Inp.width * Inp.height * Inp.N_Bits / 8];
            int[] hist = new int[256];
            for (int y = 0; y < Inp.height; y++) // =======================================
            {
                yStart = Math.Max(y - hWind, 0);
                yEnd = Math.Min(y + hWind, Inp.height - 1);
                for (int x = 0; x < Inp.width; x++) //====================================
                {
                    if (x == 0) //-----------------------------------------------
                    {
                        for (gv = 0; gv < 256; gv++) hist[gv] = 0;
                        for (y1 = yStart; y1 <= yEnd; y1++)
                            for (int xx = 0; xx <= hWind; xx++) hist[Inp.Grid[xx + y1 * Inp.width]]++;
                    }
                    else
                    {
                        int x1 = x + hWind, x2 = x - hWind - 1;
                        if (x1 < Inp.width)
                            for (y1 = yStart; y1 <= yEnd; y1++) hist[Inp.Grid[x1 + y1 * Inp.width]]++;
                        if (x2 >= 0)
                            for (y1 = yStart; y1 <= yEnd; y1++)
                            {
                                hist[Inp.Grid[x2 + y1 * Inp.width]]--;
                                if (hist[Inp.Grid[x2 + y1 * Inp.width]] < 0) return -1;
                            }
                    } //---------------- end if (x==0) ------------------------
                    int Sum = 0, nPixel = 0, prov;
                    int gvMin = Math.Max(0, Inp.Grid[x + Inp.width * y] - Toleranz),
                        gvMax = Math.Min(255, Inp.Grid[x + Inp.width * y] + Toleranz);
                    for (gv = gvMin; gv <= gvMax; gv++)
                    {
                        Sum += gv * hist[gv]; nPixel += hist[gv];
                    }
                    if (nPixel > 0) prov = (Sum + nPixel / 2) / nPixel;
                    else prov = Inp.Grid[x + Inp.width * y];
                    Sum = nPixel = 0;
                    gvMin = Math.Max(0, prov - Toleranz);
                    gvMax = Math.Min(255, prov + Toleranz);
                    for (gv = gvMin; gv <= gvMax; gv++)
                    {
                        Sum += gv * hist[gv]; nPixel += hist[gv];
                    }
                    if (nPixel > 0) Inp.Grid[x + Inp.width * y] = (byte)((Sum + nPixel / 2) / nPixel);
                    else Inp.Grid[x + Inp.width * y] = Inp.Grid[x + Inp.width * y];
                } //================== end for (int x... ======================
            } //==================== end for (int y... ========================
            return 1;
        } //********************** end SigmaFastFilter **********************************

        //public int SigmaFastFilter(FitsFile Inp, int hWind, int Toleranz)
        //// Sigma filter with doubled calculation of the output values for gray value images.
        //{
        //    int gv, y1, yEnd, yStart;
        //    Inp.Grid = new byte[Inp.Xaxis * Inp.Yaxis * Inp.N_Bits / 8];
        //    int[] hist = new int[256];
        //    for (int y = 0; y < Inp.Yaxis; y++) // =======================================
        //    {
        //        yStart = Math.Max(y - hWind, 0);
        //        yEnd = Math.Min(y + hWind, Inp.Yaxis - 1);
        //        for (int x = 0; x < Inp.Xaxis; x++) //====================================
        //        {
        //            if (x == 0) //-----------------------------------------------
        //            {
        //                for (gv = 0; gv < 256; gv++) hist[gv] = 0;
        //                for (y1 = yStart; y1 <= yEnd; y1++)
        //                    for (int xx = 0; xx <= hWind; xx++) hist[Inp.Grid[xx + y1 * Inp.Xaxis]]++;
        //            }
        //            else
        //            {
        //                int x1 = x + hWind, x2 = x - hWind - 1;
        //                if (x1 < Inp.Xaxis)
        //                    for (y1 = yStart; y1 <= yEnd; y1++) hist[Inp.Grid[x1 + y1 * Inp.Xaxis]]++;
        //                if (x2 >= 0)
        //                    for (y1 = yStart; y1 <= yEnd; y1++)
        //                    {
        //                        hist[Inp.Grid[x2 + y1 * Inp.Xaxis]]--;
        //                        if (hist[Inp.Grid[x2 + y1 * Inp.Xaxis]] < 0) return -1;
        //                    }
        //            } //---------------- end if (x==0) ------------------------
        //            int Sum = 0, nPixel = 0, prov;
        //            int gvMin = Math.Max(0, Inp.Grid[x + Inp.Xaxis * y] - Toleranz),
        //                gvMax = Math.Min(255, Inp.Grid[x + Inp.Xaxis * y] + Toleranz);
        //            for (gv = gvMin; gv <= gvMax; gv++)
        //            {
        //                Sum += gv * hist[gv]; nPixel += hist[gv];
        //            }
        //            if (nPixel > 0) prov = (Sum + nPixel / 2) / nPixel;
        //            else prov = Inp.Grid[x + Inp.Xaxis * y];
        //            Sum = nPixel = 0;
        //            gvMin = Math.Max(0, prov - Toleranz);
        //            gvMax = Math.Min(255, prov + Toleranz);
        //            for (gv = gvMin; gv <= gvMax; gv++)
        //            {
        //                Sum += gv * hist[gv]; nPixel += hist[gv];
        //            }
        //            if (nPixel > 0) Inp.Grid[x + Inp.Xaxis * y] = (byte)((Sum + nPixel / 2) / nPixel);
        //            else Inp.Grid[x + Inp.Xaxis * y] = Inp.Grid[x + Inp.Xaxis * y];
        //        } //================== end for (int x... ======================
        //    } //==================== end for (int y... ========================
        //    return 1;
        //} //********************** end SigmaFastFilter **********************************

    }
}
