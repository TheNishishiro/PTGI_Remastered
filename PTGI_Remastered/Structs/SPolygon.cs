using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    [Serializable]
    public struct SPolygon
    {
        public PTGI_StructTypes structType;
        public PTGI_ObjectTypes objectType;
        public PTGI_MaterialReflectivness reflectivnessType;

        public SLine[] Walls;
        public SPoint[] Verticies;

        public int LastWallIntersectedIndex;
        public Color Color;
        public float EmissionStrength;
        public float Density;

        public string Name;

        public bool HasValue;
        public bool IsUpdated;

    }
}
