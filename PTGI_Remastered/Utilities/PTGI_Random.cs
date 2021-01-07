using Alea;
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

        public static double NextDouble()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.NextDouble();
        }

        public static double GetRandom(ref int seed, int Xn)
        {
            int a = 1103515245;
            int c = 12345;
            int m = (int)DeviceFunction.Pow(2, 31);

            seed = (a * Xn + c) % m;

            return DeviceFunction.Abs((float)seed / (float)m);
        }

        public static double GetRandomBetween(ref int seed, double minimum, double maximum)
        {
            double random = GetRandom(ref seed, seed);
            return random * (maximum - minimum) + minimum;
        }

        public static Point GetPointInRadius(ref int seed, double radius)
        {
            double distance = GetRandomBetween(ref seed, 0, DeviceFunction.Floor(radius));
            double angleInRadians = GetRandomBetween(ref seed, 0, 360) / (2 * 3.14);

            Point pointInRadius = new Point();
            pointInRadius.SetCoords(distance * DeviceFunction.Cos(angleInRadians), distance * DeviceFunction.Sin(angleInRadians));

            return pointInRadius;
        }
    }
}
