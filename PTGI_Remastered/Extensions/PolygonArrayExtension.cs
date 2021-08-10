using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Extensions
{
    public static class PolygonArrayExtension
    {
        public static Line[] ExtractWalls(this Polygon[] polygons)
        {
            return (
                from polygon in polygons
                from wall in polygon.Walls
                select new Line()
                {
                    Coefficient = wall.Coefficient,
                    Color = polygon.Color,
                    Density = polygon.Density,
                    Destination = wall.Destination,
                    EmissionStrength = polygon.EmissionStrength,
                    HasValue = wall.HasValue,
                    ObjectType = (int)polygon.objectType,
                    ReflectivnessType = (int)polygon.reflectivnessType,
                    Source = wall.Source,
                    StructType = (int)PTGI_StructTypes.Line,
                    WasChecked = wall.WasChecked
                }).ToArray();
        }
    }
}
