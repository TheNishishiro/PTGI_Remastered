using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    [Serializable]
    public struct Polygon
    {
        public PTGI_StructTypes structType;
        public PTGI_ObjectTypes objectType;
        public PTGI_MaterialReflectivness reflectivnessType;

        public Line[] Walls;
        public Point[] Verticies;

        public int LastWallIntersectedIndex;
        public Color Color;
        public float EmissionStrength;
        public float Density;

        public string Name;

        public bool HasValue;

        public void CreateRectangle(Point[] Verticies)
        {
            if (Verticies.Length < 2)
                throw new Exception("Not enough verticies to construct rectangle");

            Walls = new Line[4];
            var topLeft = Verticies[0];
            var bottomRight = Verticies[1];

            var topRight = new Point();
            topRight.SetCoords(bottomRight.X, topLeft.Y);
            var bottomLeft = new Point();
            bottomLeft.SetCoords(topLeft.X, bottomRight.Y);

            Walls[0].Setup(topLeft, topRight);
            Walls[1].Setup(topRight, bottomRight);
            Walls[2].Setup(bottomRight, bottomLeft);
            Walls[3].Setup(bottomLeft, topLeft);
            structType = PTGI_StructTypes.Rectangle;
        }

        public void Setup(Point[] Verticies, PTGI_ObjectTypes objectType, PTGI_MaterialReflectivness reflectivnessType, Color color, float EmissionStrength, float Density)
        {
            if (Verticies.Length > 2)
                CreateWalls(Verticies);
            else
                CreateRectangle(Verticies);

            this.Verticies = new Point[Verticies.Length];
            Verticies.CopyTo(this.Verticies, 0);

            this.objectType = objectType;
            this.reflectivnessType = reflectivnessType;
            this.Color = color;
            this.EmissionStrength = EmissionStrength;
            this.Density = Density;
            HasValue = true;
        }

        public void ResetWallCheckedStatus()
        {
            for(int i = 0; i < Walls.Length; i++)
                Walls[i].MarkAsReadyForIntersection();
        }


        public void CreateWalls(Point[] Verticies)
        {
            var verticiesCount = Verticies.Length;
            if (verticiesCount < 3)
                throw new Exception("Not enough verticies to construct polygon");

            Walls = new Line[verticiesCount];
            for (int i = 0, wallIndex = 0; i < verticiesCount - 1; i++, wallIndex++)
            {
                Walls[wallIndex] = new Line();
                Walls[wallIndex].Setup(Verticies[i], Verticies[i+1]);
            }
            Walls[verticiesCount - 1] = new Line();
            Walls[verticiesCount - 1].Setup(Verticies[verticiesCount - 1], Verticies[0]);
            structType = PTGI_StructTypes.Polygon;
        }

        public Point GetIntersectionPoint(Line line, Line wallToIgnore)
        {
            var intersectionPoint = new Point();
            intersectionPoint.HasValue = 0;

            var closestDistance = float.MaxValue;
            var wallsCount = Walls.Count();

            for(var wallIndex = 0; wallIndex < wallsCount; wallIndex++)
            {
                if (Walls[wallIndex].WasChecked == 1 || (wallToIgnore.HasValue == 1 && Walls[wallIndex].IsEqualTo(wallToIgnore)))
                    continue;

                var newIntersection = Walls[wallIndex].GetIntersection(line, wallToIgnore);
                if (newIntersection.HasValue != 1) continue;
                
                var distance = line.Source.GetDistance(newIntersection);
                if (!(closestDistance > distance)) continue;
                
                intersectionPoint = newIntersection;
                closestDistance = distance;
                LastWallIntersectedIndex = wallIndex;
            }

            return intersectionPoint;
        }
    }
}
