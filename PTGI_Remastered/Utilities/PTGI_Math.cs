using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTGI_Remastered.Classes;

namespace PTGI_Remastered.Utilities
{
    public class PTGI_Math
    {
        public static SPoint Convert1dIndexTo2d(Bitmap bitmap, int threadId)
        {
            var col = threadId / bitmap.Width;
            var row = threadId % bitmap.Width;

            var raySource = new SPoint();
            raySource.SetCoords(row, col);
            return raySource;
        }
        
        public static Point Convert1dIndexTo2dPointClass(Bitmap bitmap, int threadId)
        {
            var col = threadId / bitmap.Width;
            var row = threadId % bitmap.Width;

            return new Point(row, col);
        }

        public static int Convert2dIndexTo1d(float x, float y, Bitmap bitmap)
        {
            return (int)y * bitmap.Width + (int)x;
        }

        public static int Convert3dIndexTo1d(int x, int y, int z, int width, int height)
        {
            return (z * width * height) + (y * width) + x;
        }
        
        public static float Pow(float x, float y)
        {
            float result = 1;
            for (var i = 0; i < y; i++)
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
