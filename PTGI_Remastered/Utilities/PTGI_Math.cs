using ILGPU;
using ILGPU.Algorithms;
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
        public static Point Convert1dIndexTo2d(Bitmap bitmap, int threadId)
        {
            int col = threadId / bitmap.Width;
            int row = threadId % bitmap.Width;

            
            Point raySource = new Point();
            raySource.SetCoords(row, col);
            return raySource;
        }

        public static int Convert2dIndexTo1d(float x, float y, Bitmap bitmap)
        {
            return (int)y * bitmap.Width + (int)x;
        }

        public static float Pow(float x, float y)
        {
            float result = 1;
            for (int i = 0; i < y; i++)
                result *= x;
            return result;
        }

        public static float PowFloat(float x, float y)
        {
            return XMath.Exp(y * XMath.Log(x, 10));
        }

        public static float Modulo(float x, float y)
        {
            if (y == 0.0f ||
                XMath.IsInfinity(x) ||
                XMath.IsNaN(x) ||
                XMath.IsNaN(y))
                return float.NaN;

            if (XMath.IsInfinity(y))
                return x;

            var xDivY = XMath.Abs(x * XMath.Rcp(y));
            var result = (xDivY - XMath.Floor(xDivY)) * XMath.Abs(y);

            if (x < 0.0f)
                return -result;
            return result;
        }
    }
}
