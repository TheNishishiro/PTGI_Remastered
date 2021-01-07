using Alea;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct Point
    {
        public double X;
        public double Y;
        public bool HasValue;

        public void SetCoords(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            HasValue = true;
        }

        public Point SubstractNew(Point point)
        {
            Point newPoint = new Point();
            newPoint.SetCoords(X - point.X, Y - point.Y);

            return newPoint;
        }

        public void Substract(Point point)
        {
            SetCoords(X - point.X, Y - point.Y);
        }

        public Point AddNew(Point point)
        {
            Point newPoint = new Point();
            newPoint.SetCoords(X + point.X, Y + point.Y);

            return newPoint;
        }

        public void Add(Point point)
        {
            SetCoords(X + point.X, Y + point.Y);
        }

        public Point MultiplyNew(double factor)
        {
            Point newPoint = new Point();
            newPoint.SetCoords(X * factor, Y * factor);

            return newPoint;
        }

        public void Multiply(double factor)
        {
            SetCoords(X * factor, Y * factor);
        }

        public Point MultiplyNew(Point point)
        {
            Point newPoint = new Point();
            newPoint.SetCoords(X * point.X, Y * point.Y);

            return newPoint;
        }

        public void Multiply(Point point)
        {
            SetCoords(X * point.X, Y * point.Y);
        }

        public bool IsEqualTo(Point point)
        {
            return X == point.X && Y == point.Y;
        }

        public void Normalize()
        {
            var newPoint = GetNormalized();
            SetCoords(newPoint.X, newPoint.Y);
        }

        public Point GetDirection(Point Destination)
        {
            return Destination.SubstractNew(this);
        }

        public Point GetNormalized()
        {
            Point newPoint = new Point();
            var length = X * X + Y * Y;
            newPoint.SetCoords(X / DeviceFunction.Sqrt(length), Y / DeviceFunction.Sqrt(length));
            return newPoint;
        }

        public double DotProduct(Point point)
        {
            return X * point.Y + X * point.Y;
        }

        public double GetDistance(Point destination)
        {
            return DeviceFunction.Sqrt(DeviceFunction.Pow(destination.X - X, 2) + DeviceFunction.Pow(destination.Y - Y, 2));
        }

        public double Magnitude()
        {
            return DeviceFunction.Sqrt(X * X + Y * Y);
        }

        public bool LiesInLine(Line line)
        {
            double epsilon = 0.001;
            return DeviceFunction.Abs(Y - (line.Coefficient.A * X + line.Coefficient.B)) < epsilon;
        }

        public bool LiesInObject(Polygon obstacle)
        {
            switch (obstacle.structType)
            {
                case PTGI_StructTypes.Polygon:
                    return LiesInPolygon(obstacle);
                case PTGI_StructTypes.Rectangle:
                    return LiesInRectangle(obstacle);
                default:
                    return false;
            }
        }

        private bool LiesInRectangle(Polygon obstacle)
        {
            if (X > obstacle.Walls[0].Source.X && Y > obstacle.Walls[0].Source.Y &&
                X < obstacle.Walls[1].Destination.X && Y < obstacle.Walls[1].Destination.Y)
                return true;
            return false;
        }
        
        private bool LiesInPolygon(Polygon obstacle)
        {
            bool result = false;
            int verticiesCount = obstacle.Verticies.Length;
            int j = verticiesCount - 1;

            for (int i = 0; i < verticiesCount; i++)
            {
                if (obstacle.Verticies[i].Y < Y && obstacle.Verticies[j].Y >= Y || obstacle.Verticies[j].Y < Y && obstacle.Verticies[i].Y >= Y)
                {
                    if (obstacle.Verticies[i].X + (Y - obstacle.Verticies[i].Y) / (obstacle.Verticies[j].Y - obstacle.Verticies[i].Y) * (obstacle.Verticies[j].X - obstacle.Verticies[i].X) < X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }

            return result;
        }
    }
}
