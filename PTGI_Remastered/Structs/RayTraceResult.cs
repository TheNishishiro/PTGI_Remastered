using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct RayTraceResult
    {
        public Color pixelColor;
        public Point[] rayTrace;
        public int[] rayGridMovement;
        public bool collectRayTrace;
        private int rayTraceId;
        private int gridPointId;

        public void AddRayDebugPoint(Point point)
        {
            if (collectRayTrace && rayTraceId < rayTrace.Length)
            {
                rayTrace[rayTraceId] = point;
                rayTraceId++;
            }
        }

        public void AddGridDebugPoint(int cellId)
        {
            if (collectRayTrace && gridPointId < rayGridMovement.Length)
            {
                rayGridMovement[gridPointId] = cellId;
                gridPointId++;
            }
        }
    }
}
