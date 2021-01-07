using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    struct PolygonContainer
    {
        public int PolygonCount;
        public PTGI_ObjectTypes[] ObjectTypes;
        public PTGI_StructTypes[] StructType;
        public PTGI_MaterialReflectivness[] ReflectivnessType;
        public Color[] Color;
        public double[] EmissionStrength;
        public double[] Density;
        public bool[] HasValue;
        public void Setup(Polygon[] collisionObjects, Line[][] walls, Point[][] verticies)
        {
            HasValue = new bool[PolygonCount];
            EmissionStrength = new double[PolygonCount];
            Density = new double[PolygonCount];
            Color = new Color[PolygonCount];
            ReflectivnessType = new PTGI_MaterialReflectivness[PolygonCount];
            StructType = new PTGI_StructTypes[PolygonCount];
            ObjectTypes = new PTGI_ObjectTypes[PolygonCount];

            for (int i = 0; i < PolygonCount; i++)
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
        public void CUDA_Copy(Polygon[] CUDAcollisionObjects, Line[][] walls, Point[][] verticies)
        {
            for (int i = 0; i < PolygonCount; i++)
            {
                CUDAcollisionObjects[i] = new Polygon();
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
