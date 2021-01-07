using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Utilities
{
    public enum PTGI_StructTypes
    {
        Point = 1,
        Line = 2,
        Polygon = 3,
        Rectangle = 4
    }

    public enum PTGI_ObjectTypes
    {
        Solid = 1,
        LightSource = 2
    }

    public enum PTGI_MaterialReflectivness
    {
        Rough = 1,
        Reflective = 2,
        Mirror = 3,
        Transparent = 4
    }
}
