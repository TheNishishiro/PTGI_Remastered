using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct SLine
    {
        public SPoint Source;
        public SPoint Destination;
        public SLineCoefficient Coefficient;
        public byte HasValue;
        public byte WasChecked;

        public int StructType;
        public int ObjectType;
        public int ReflectivnessType;
        public Color Color;
        public float EmissionStrength;
        public float Density;

        public SLine Setup(SPoint source, SPoint destination)
        {
            Source = source;
            Destination = destination;
            Coefficient = GetCoefficients();
            HasValue = 1;
            return this;
        }

        private SLineCoefficient GetCoefficients()
        {
            var lineComponents = new SLineCoefficient();

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

        public SLineNormals GetNormals()
        {
            var dx = Destination.X - Source.X;
            var dy = Destination.Y - Source.Y;

            var lineNormals = new SLineNormals();
            lineNormals.NormalDown = new SPoint();
            lineNormals.NormalDown.SetCoords(-dy, dx);

            lineNormals.NormalUp = new SPoint();
            lineNormals.NormalUp.SetCoords(dy, -dx);

            return lineNormals;
        }

        public bool IsEqualTo(SLine sLine)
        {
            return Source.IsEqualTo(sLine.Source) && Destination.IsEqualTo(sLine.Destination);
        }

        private SLineNormals MoveNormalsRelative(SLineNormals sLineNormals, SPoint intersection, float reflectionArea)
        {
            var movedLineNormals = new SLineNormals();
            movedLineNormals.NormalDown = sLineNormals.NormalDown.MultiplyNew(reflectionArea).AddNew(intersection);
            movedLineNormals.NormalUp = sLineNormals.NormalUp.MultiplyNew(reflectionArea).AddNew(intersection);

            return movedLineNormals;
        }

        public SPoint GetShiftedClosestNormal(SPoint source, SPoint intersection, float reflectionArea)
        {
            var lineNormals = GetNormals().Normalize();
            var lineShiftedNormals = MoveNormalsRelative(lineNormals, intersection, reflectionArea);
            return source.GetDistance(lineShiftedNormals.NormalDown) <= source.GetDistance(lineShiftedNormals.NormalUp) ? lineShiftedNormals.NormalDown : lineShiftedNormals.NormalUp;
        }

        public SPoint GetDirection()
        {
            var point = new SPoint();
            point.SetCoords(Destination.X, Destination.Y);
            point.Substract(Source);
            return point;
        }

        /// <summary>
        /// Computes point of intersection between two lines
        /// </summary>
        /// <param name="intersectingSLine">line to check intersection with</param>
        /// <param name="ignoredSLine">line to ignore during intersection</param>
        /// <returns>if point exists return point with HasValue set to true</returns>
        public SPoint GetIntersection(SLine intersectingSLine, SLine ignoredSLine)
        {
            var intersectionPoint = new SPoint();

            if ((ignoredSLine.HasValue == 1 && ignoredSLine.IsEqualTo(this)) || WasChecked == 1)
                return intersectionPoint;
            var delta = (Source.X - Destination.X) * (intersectingSLine.Source.Y - intersectingSLine.Destination.Y) - (Source.Y - Destination.Y) * (intersectingSLine.Source.X - intersectingSLine.Destination.X);
            if(delta == 0)
                return intersectionPoint;

            var t = ((Source.X - intersectingSLine.Source.X) * (intersectingSLine.Source.Y - intersectingSLine.Destination.Y) - (Source.Y - intersectingSLine.Source.Y) * (intersectingSLine.Source.X - intersectingSLine.Destination.X)) / delta;
            var u = -((Source.X - Destination.X) * (Source.Y - intersectingSLine.Source.Y) - (Source.Y - Destination.Y) * (Source.X - intersectingSLine.Source.X)) / delta;

            if(t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersectionPoint.SetCoords(Source.X + t * (Destination.X - Source.X), Source.Y + t * (Destination.Y - Source.Y));
            }
            
            return intersectionPoint;
        }
    }
}
