using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Utilities
{
    public class PTGI_Math
    {
        public static Point GetRaySourceFromThreadIndex(ref Bitmap bitmap, int threadId)
        {
            // Convert 1D index to 2D index
            int row = threadId % bitmap.Width;
            int col = threadId / bitmap.Width;

            Point raySource = new Point();
            raySource.SetCoords(row, col);

            return raySource;
        }
    }
}
