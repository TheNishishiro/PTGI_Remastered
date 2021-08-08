using ILGPU;
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
        public static Point GetRaySourceFromThreadIndex(Bitmap bitmap, int threadId)
        {
            // Convert 1D index to 2D index
            int row = threadId % bitmap.Width;
            int col = threadId / bitmap.Width;

            
            Point raySource = new Point();
            raySource.SetCoords(row, col);
            return raySource;
        }

        public static float Pow(float x, float y)
        {
            float result = 1;
            for (int i = 0; i < y; i++)
                result *= x;
            return result;
        }
    }
}
