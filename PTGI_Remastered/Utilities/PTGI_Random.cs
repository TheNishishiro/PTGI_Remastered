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
    public class PTGI_Random
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }

        public static int Next(int x, int y)
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next(x, y);
        }

        public static float Nextfloat()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return (float)inst.NextDouble();
        }

        public static float GetRandom(Index1 index, ArrayView<int> seed, int Xn)
        {
            int a = 1103515245;
            int c = 12345;
            int m = (int)PTGI_Math.Pow(2, 31);

            seed[index] = (a * Xn + c) % m;

            return XMath.Abs((float)seed[index] / (float)m);
        }

        public static float GetRandomBetween(Index1 index, ArrayView<int> seed, float minimum, float maximum)
        {
            float random = GetRandom(index, seed, seed[index]);
            return random * (maximum - minimum) + minimum;
        }

        public static Point GetPointInRadius(Index1 index, ArrayView<int> seed, float radius)
        {
            float distance = GetRandomBetween(index, seed, 0, XMath.Floor(radius));
            float angleInRadians = GetRandomBetween(index, seed, 0, 360) / (2 * 3.14f);

            Point pointInRadius = new Point();
            pointInRadius.SetCoords(distance * XMath.Cos(angleInRadians), distance * XMath.Sin(angleInRadians));

            return pointInRadius;
        }
    }
}
