using Alea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    [Serializable]
    public struct Line
    {
        public Point Source;
        public Point Destination;
        public LineCoefficient Coefficient;
        public bool HasValue;
        public bool WasChecked;

        public void Setup(Point Source, Point Destination)
        {
            this.Source = Source;
            this.Destination = Destination;
            GetCoefficients();
            HasValue = true;
        }

        private LineCoefficient GetCoefficients()
        {
            LineCoefficient lineComponents = new LineCoefficient();

            lineComponents.A = (Destination.Y - Source.Y) / (Destination.X - Source.X);
            lineComponents.B = Source.Y - lineComponents.A * Source.X;

            return lineComponents;
        }

        public void MarkAsCheckedForIntersection()
        {
            WasChecked = true;
        }

        public void MarkAsReadyForIntersection()
        {
            WasChecked = false;
        }

        public double GetLength()
        {
            return Source.GetDistance(Destination);
        }

        public LineNormals GetNormals()
        {
            double dx = Destination.X - Source.X;
            double dy = Destination.Y - Source.Y;

            LineNormals lineNormals = new LineNormals();
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

        private LineNormals MoveNormalsRelative(LineNormals lineNormals, Point intersection, double reflectionArea)
        {
            LineNormals movedLineNormals = new LineNormals();
            movedLineNormals.NormalDown = lineNormals.NormalDown.MultiplyNew(reflectionArea).AddNew(intersection);
            movedLineNormals.NormalUp = lineNormals.NormalUp.MultiplyNew(reflectionArea).AddNew(intersection);

            return movedLineNormals;
        }

        public Point GetShiftedClosestNormal(Point source, Point intersection, double reflectionArea)
        {
            var lineNormals = GetNormals().Normalize();
            var lineShiftedNormals = MoveNormalsRelative(lineNormals, intersection, reflectionArea);
            if (source.GetDistance(lineShiftedNormals.NormalDown) <= source.GetDistance(lineShiftedNormals.NormalUp))
                return lineShiftedNormals.NormalDown;
            else
                return lineShiftedNormals.NormalUp;
        }

        public Point GetDirection()
        {
            Point point = new Point();
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
            Point intersectionPoint = new Point();

            if ((ignoredLine.HasValue && ignoredLine.IsEqualTo(this)) || WasChecked)
                return intersectionPoint;
            //MarkAsCheckedForIntersection();
            double delta = (Source.X - Destination.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - Destination.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X);
            if(delta == 0)
                return intersectionPoint;

            double t = ((Source.X - intersectingLine.Source.X) * (intersectingLine.Source.Y - intersectingLine.Destination.Y) - (Source.Y - intersectingLine.Source.Y) * (intersectingLine.Source.X - intersectingLine.Destination.X)) / delta;
            double u = -((Source.X - Destination.X) * (Source.Y - intersectingLine.Source.Y) - (Source.Y - Destination.Y) * (Source.X - intersectingLine.Source.X)) / delta;

            if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersectionPoint.SetCoords(Source.X + t * (Destination.X - Source.X), Source.Y + t * (Destination.Y - Source.Y));
            }
            
            return intersectionPoint;
        }
    }
}
