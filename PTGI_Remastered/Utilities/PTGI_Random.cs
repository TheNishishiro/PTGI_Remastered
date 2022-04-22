using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime;

namespace PTGI_Remastered.Utilities
{
    public class PTGI_Random
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public static int Next()
        {
            var inst = _local;
            if (inst != null) return inst.Next();
            int seed;
            lock (_global) seed = _global.Next();
            _local = inst = new Random(seed);
            return inst.Next();
        }

        public static int Next(int x, int y)
        {
            var inst = _local;
            if (inst != null) return inst.Next(x, y);
            int seed;
            lock (_global) seed = _global.Next();
            _local = inst = new Random(seed);
            return inst.Next(x, y);
        }

        public static float Nextfloat()
        {
            var inst = _local;
            if (inst != null) return (float) inst.NextDouble();
            int seed;
            lock (_global) seed = _global.Next();
            _local = inst = new Random(seed);
            return (float)inst.NextDouble();
        }

        public static float GetRandom(Index1D index, ArrayView1D<int, Stride1D.Dense> seed, int xn)
        {
            const int a = 1103515245;
            const int c = 12345;
            var m = (int)MathF.Pow(2, 31);

            seed[index] = (a * xn + c) % m;

            return IntrinsicMath.Abs((float)seed[index] / (float)m);
        }

        public static float GetRandomBetween(Index1D index, ArrayView1D<int, Stride1D.Dense> seed, float minimum, float maximum)
        {
            var random =  GetRandom(index, seed, seed[index]);
            return random * (maximum - minimum) + minimum;
        }

        public static Point GetPointInRadius(Index1D index, ArrayView1D<int, Stride1D.Dense> seed, float radius)
        {
            var distance = GetRandomBetween(index, seed, 0, MathF.Floor(radius));
            var angleInRadians = GetRandomBetween(index, seed, 0, 360) / (2 * 3.14f);

            var pointInRadius = new Point();
            pointInRadius.SetCoords(distance * MathF.Cos(angleInRadians), distance * MathF.Sin(angleInRadians));

            return pointInRadius;
        }
    }
}
