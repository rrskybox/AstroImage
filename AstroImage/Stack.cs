using System.Linq;

namespace AstroImage
{
    public class Stack
    {
        public FitsFile FitsStack { get; set; }

        public Stack(FitsFile[] fsSet)
        {
            FitsStack = new FitsFile(fsSet[0]);
            if (fsSet.Length > 1)
            {
                int vLength = FitsStack.Xaxis * FitsStack.Yaxis;
                for (int i = 0; i < vLength; i++)
                {
                    for (int f=1; f<fsSet.Length; f++)
                        FitsStack.FITS_Vector[i] += fsSet[f].FITS_Vector[i];
                }
                for (int i = 0; i < vLength; i++)
                    FitsStack.FITS_Vector[i] /= fsSet.Count();
                for (int iy = 0; iy < FitsStack.Yaxis; iy++)
                    for (int ix = 0; ix < FitsStack.Xaxis; ix++)
                        FitsStack.FITS_Array[ix, iy] = (ushort)FitsStack.FITS_Vector[ix + iy * FitsStack.Xaxis];
            }
            return;
        }
    }
}
