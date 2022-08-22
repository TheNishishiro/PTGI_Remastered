using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    internal struct PolygonContainer
    {
        public int PolygonCount;
        public PTGI_ObjectTypes[] ObjectTypes;
        public PTGI_StructTypes[] StructType;
        public PTGI_MaterialReflectivness[] ReflectivnessType;
        public Color[] Color;
        public float[] EmissionStrength;
        public float[] Density;
        public bool[] HasValue;
        public void Setup(SPolygon[] collisionObjects, SLine[][] walls, SPoint[][] verticies)
        {
            HasValue = new bool[PolygonCount];
            EmissionStrength = new float[PolygonCount];
            Density = new float[PolygonCount];
            Color = new Color[PolygonCount];
            ReflectivnessType = new PTGI_MaterialReflectivness[PolygonCount];
            StructType = new PTGI_StructTypes[PolygonCount];
            ObjectTypes = new PTGI_ObjectTypes[PolygonCount];

            for (var i = 0; i < PolygonCount; i++)
            {
                walls[i] = collisionObjects[i].Walls;
                verticies[i] = collisionObjects[i].Verticies;
                HasValue[i] = collisionObjects[i].HasValue;
                Density[i] = collisionObjects[i].Density;
                EmissionStrength[i] = collisionObjects[i].EmissionStrength;
                Color[i] = collisionObjects[i].Color;
                ReflectivnessType[i] = collisionObjects[i].reflectivnessType;
                StructType[i] = collisionObjects[i].structType;
                ObjectTypes[i] = collisionObjects[i].objectType;
            }
        }
        public void CUDA_Copy(SPolygon[] CUDAcollisionObjects, SLine[][] walls, SPoint[][] verticies)
        {
            for (var i = 0; i < PolygonCount; i++)
            {
                CUDAcollisionObjects[i] = new SPolygon();
                CUDAcollisionObjects[i].Walls = walls[i];
                CUDAcollisionObjects[i].EmissionStrength = EmissionStrength[i];
                CUDAcollisionObjects[i].Verticies = verticies[i];
                CUDAcollisionObjects[i].objectType = ObjectTypes[i];
                CUDAcollisionObjects[i].reflectivnessType = ReflectivnessType[i];
                CUDAcollisionObjects[i].HasValue = HasValue[i];
                CUDAcollisionObjects[i].structType = StructType[i];
                CUDAcollisionObjects[i].Color = Color[i];
                CUDAcollisionObjects[i].Density = Density[i];
            }
        }
    }
}
