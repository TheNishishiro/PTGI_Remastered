using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct Line
    {
        public Point Source;
        public Point Destination;
        public LineCoefficient Coefficient;
        public byte HasValue;
        public byte WasChecked;

        public int StructType;
        public int ObjectType;
        public int ReflectivnessType;
        public Color Color;
        public float EmissionStrength;
        public float Density;

        public Line Setup(Point source, Point destination)
        {
            Source = source;
            Destination = destination;
            Coefficient = GetCoefficients();
            HasValue = 1;
            return this;
        }

        private LineCoefficient GetCoefficients()
        {
            var lineComponents = new LineCoefficient();

            lineComponents.A = (Destination.Y - Source.Y) / (Destination.X - Source.X);
            lineComponents.B = Source.Y - lineComponents.A * Source.X;

            return lineComponents;
        }

        public void MarkAsCheckedForIntersection()
        {
            WasChecked = 1;
        }

        public void MarkAsReadyForIntersection()
        {
            WasChecked = 0;
        }

        public float GetLength()
        {
            return Source.GetDistance(Destination);
        }

        public LineNormals GetNormals()
        {
            var dx = Destination.X - Source.X;
            var dy = Destination.Y - Source.Y;

            var lineNormals = new LineNormals();
            lineNormals.NormalDown = new Point();
            lineNormals.NormalDown.SetCoords(-dy, dx);

            lineNormals.NormalUp = new Point();
            lineNormals.NormalUp.SetCoords(dy, -dx);

            return lineNormals;
        }

        public bool IsEqualTo(Line line)
        {
            return Source.IsEqualTo(line.Source) && Destination.IsEqualTo(line.Destination);
        }

        private LineNormals MoveNormalsRelative(LineNormals lineNormals, Point intersection, float reflectionArea)
        {
            var movedLineNormals = new LineNormals();
            movedLineNormals.NormalDown = lineNormals.NormalDown.MultiplyNew(reflectionArea).AddNew(intersection);
            movedLineNormals.NormalUp = lineNormals.NormalUp.MultiplyNew(reflectionArea).AddNew(intersection);

            return movedLineNormals;
        }

        public Point GetShiftedClosestNormal(Point source, Point intersection, float reflectionArea)
        {
            var lineNormals = GetNormals().Normalize();
            var lineShiftedNormals = MoveNormalsRelative(lineNormals, intersection, reflectionArea);
            return source.GetDistance(lineShiftedNormals.NormalDown) <= source.GetDistance(lineShiftedNormals.NormalUp) ? lineShiftedNormals.NormalDown : lineShiftedNormals.NormalUp;
        }

        public Point GetDirection()
        {
            var point = new Point();
            point.SetCoords(Destination.X, Destination.Y);
            point.Substract(Source);
            return point;
        }

        /// <summary>
        /// Computes point of intersection between two lines
        /// </summary>
        /// <param name="intersectingLine">line to check intersection with</param>
        /// <param name="ignoredLine">line to ignore during intersection</param>
        /// <returns>if point exists return point with HasValue set to true</returns>
        public Point GetIntersection(Line intersectingLine, Line ignoredLine)
        {
            var intersectionPoint = new Point();

            if ((ignoredLine.HasValue == 1 && ignoredLine.IsEqualTo(this)) || WasChecked == 1)
                return intersectionPoint;
            var delta = (Source.X - Destination.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - Destination.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X);
            if(delta == 0)
                return intersectionPoint;

            var t = ((Source.X - intersectingLine.Source.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - intersectingLine.Source.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X)) / delta;
            var u = -((Source.X - Destination.X) * (Source.Y - intersectingLine.Source.Y) - (Source.Y - Destination.Y) * (Source.X - intersectingLine.Source.X)) / delta;

            if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersectionPoint.SetCoords(Source.X + t * (Destination.X - Source.X), Source.Y + t * (Destination.Y - Source.Y));
            }
            
            return intersectionPoint;
        }
    }
}
