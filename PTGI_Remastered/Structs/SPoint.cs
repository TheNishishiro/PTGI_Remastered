using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTGI_Remastered.Classes;

namespace PTGI_Remastered.Structs
{
    [Serializable]
    public struct SPoint
    {
        public float X;
        public float Y;
        public byte HasValue;

        public void SetCoords(float x, float y)
        {
            this.X = x;
            this.Y = y;
            HasValue = 1;
        }

        public SPoint SubstractNew(SPoint sPoint)
        {
            var newPoint = new SPoint();
            newPoint.SetCoords(X - sPoint.X, Y - sPoint.Y);

            return newPoint;
        }

        public void Substract(SPoint sPoint)
        {
            SetCoords(X - sPoint.X, Y - sPoint.Y);
        }

        public SPoint AddNew(SPoint sPoint)
        {
            var newPoint = new SPoint();
            newPoint.SetCoords(X + sPoint.X, Y + sPoint.Y);

            return newPoint;
        }

        public void Add(SPoint sPoint)
        {
            SetCoords(X + sPoint.X, Y + sPoint.Y);
        }

        public SPoint MultiplyNew(float factor)
        {
            var newPoint = new SPoint();
            newPoint.SetCoords(X * factor, Y * factor);

            return newPoint;
        }

        public void Multiply(float factor)
        {
            SetCoords(X * factor, Y * factor);
        }

        public SPoint MultiplyNew(SPoint sPoint)
        {
            var newPoint = new SPoint();
            newPoint.SetCoords(X * sPoint.X, Y * sPoint.Y);

            return newPoint;
        }

        public void Multiply(SPoint sPoint)
        {
            SetCoords(X * sPoint.X, Y * sPoint.Y);
        }

        public bool IsEqualTo(SPoint sPoint)
        {
            return X == sPoint.X && Y == sPoint.Y;
        }

        public void Normalize()
        {
            var newPoint = GetNormalized();
            SetCoords(newPoint.X, newPoint.Y);
        }

        public SPoint GetDirection(SPoint Destination)
        {
            return Destination.SubstractNew(this);
        }

        public SPoint GetNormalized()
        {
            var newPoint = new SPoint();
            var length = X * X + Y * Y;
            newPoint.SetCoords(X / XMath.Sqrt(length), Y / XMath.Sqrt(length));
            return newPoint;
        }

        public float DotProduct(SPoint sPoint)
        {
            return X * sPoint.Y + X * sPoint.Y;
        }

        public float GetDistance(SPoint destination)
        {
            return XMath.Sqrt((destination.X - X) * (destination.X - X)  + (destination.Y - Y) * (destination.Y - Y));
        }

        public float Magnitude()
        {
            return XMath.Sqrt(X * X + Y * Y);
        }

        public bool LiesInLine(SLine sLine)
        {
            const float epsilon = 0.001f;
            return XMath.Abs(Y - (sLine.Coefficient.A * X + sLine.Coefficient.B)) < epsilon;
        }

        public bool LiesInObject(SPolygon obstacle)
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

        private bool LiesInRectangle(SPolygon obstacle)
        {
            return X >= obstacle.Walls[0].Source.X && Y >= obstacle.Walls[0].Source.Y &&
                   X <= obstacle.Walls[1].Destination.X && Y <= obstacle.Walls[1].Destination.Y;
        }
        
        private bool LiesInPolygon(SPolygon obstacle)
        {
            var result = false;
            var verticiesCount = obstacle.Verticies.Length;
            var j = verticiesCount - 1;

            for (var i = 0; i < verticiesCount; i++)
            {
                if ((obstacle.Verticies[i].Y < Y && obstacle.Verticies[j].Y >= Y || obstacle.Verticies[j].Y < Y && obstacle.Verticies[i].Y >= Y) && obstacle.Verticies[i].X + (Y - obstacle.Verticies[i].Y) / (obstacle.Verticies[j].Y - obstacle.Verticies[i].Y) * (obstacle.Verticies[j].X - obstacle.Verticies[i].X) < X)
                {
                    result = !result;
                }
                j = i;
            }

            return result;
        }

        
    }
}
