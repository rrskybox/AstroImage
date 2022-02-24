using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroImage
{
    public class Stack
    {
        public static FitsFile StraightStack(FitsFile[] fsSet)
        {
            FitsFile iStack = new FitsFile(fsSet[0]);
            if (fsSet.Count() > 1)
            {
                for (int fs = 1; fs < fsSet.Count(); fs++)
                {
                    for (int iy = 0; iy < fsSet[fs].Yaxis; iy++)
                        for (int ix = 0; ix < fsSet[fs].Xaxis; ix++)
                            iStack.FITSArray[ix, iy] += fsSet[fs].FITSArray[ix, iy];
                }
                for (int iy = 0; iy < iStack.Yaxis; iy++)
                    for (int ix = 0; ix < iStack.Xaxis; ix++)
                        iStack.FITSArray[ix, iy] /= (ushort)fsSet.Count();
            }
            return iStack;
        }

        public static FitsFile StraightStack(FitsFile[] fsSet,int count)
        {
            FitsFile iStack = new FitsFile(fsSet[0]);
            if (fsSet.Count() > 1)
            {
                for (int fs = 1; fs < count; fs++)
                {
                    for (int iy = 0; iy < fsSet[fs].Yaxis; iy++)
                        for (int ix = 0; ix < fsSet[fs].Xaxis; ix++)
                            iStack.FITSArray[ix, iy] += fsSet[fs].FITSArray[ix, iy];
                }
                for (int iy = 0; iy < iStack.Yaxis; iy++)
                    for (int ix = 0; ix < iStack.Xaxis; ix++)
                        iStack.FITSArray[ix, iy] /= (ushort)fsSet.Count();
            }
            return iStack;
        }
    }
}
