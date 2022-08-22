using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime;
using PTGI_Remastered.Classes;

namespace PTGI_Remastered.Utilities
{
    public struct NextRay
    {
        public SPoint Destination;
        public bool SwapDensity;
        public bool HasValue;

        public void Setup(SPoint destination, bool swapDensity)
        {
            Destination = destination;
            SwapDensity = swapDensity;
            HasValue = true;
        }
    }

    public class TraceRayUtility
    {
        
        public static NextRay NextRayDirection(Index1D index, ArrayView1D<int, Stride1D.Dense> seedArray, SLine collisionObject, SLine wallToIgnore, SPoint raySource, SPoint intersection, float reflectionArea, bool swapDensity)
        {
            const float reflectFromGlassChanceThreshold = 0.15f;
            const int directionRayOffset = 1000;
            var nextRay = new NextRay();

            var closestNormal = wallToIgnore.GetShiftedClosestNormal(raySource, intersection, reflectionArea);
            var reflectFromGlassChance = PTGI_Random.GetRandomBetween(index, seedArray, 0, 1);

            var isReflectiveOrReflectFromGlass = collisionObject.ReflectivnessType != 4 || reflectFromGlassChance < reflectFromGlassChanceThreshold;
            if (!isReflectiveOrReflectFromGlass) return nextRay;
            nextRay = collisionObject.ReflectivnessType switch
            {
                3 => ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity),
                2 => ProcessSemiRough(index, seedArray, directionRayOffset, closestNormal, raySource, intersection, reflectionArea, swapDensity),
                _ => ProcessRough(index, seedArray, directionRayOffset, closestNormal, intersection, swapDensity)
            };

            return nextRay;
        }

        
        private static NextRay ProcessRough(Index1D index, ArrayView1D<int, Stride1D.Dense> seedArray, float directionRayOffset, SPoint closestNormal, SPoint intersection, bool swapDensity)
        {
            var nextRay = new NextRay();

            var newRayDirection = PTGI_Random.GetPointInRadius(index, seedArray, closestNormal.GetDistance(intersection));
            newRayDirection.Add(closestNormal);

            var direction = intersection.GetDirection(newRayDirection);
            direction.Normalize();
            direction.Multiply(directionRayOffset);

            newRayDirection.Add(direction);

            nextRay.Setup(newRayDirection, swapDensity);

            return nextRay;
        }

        
        private static NextRay ProcessSmooth(SPoint closestNormal, float directionRayOffset, SPoint raySource, SPoint intersection, bool swapDensity)
        {
            var nextRay = new NextRay();

            var direction = raySource.GetDirection(intersection);
            direction.Normalize();

            var reflectionPoint = new SPoint();
            var dotProductResult = direction.DotProduct(closestNormal);
            reflectionPoint.SetCoords(
                direction.X - 2 * dotProductResult * closestNormal.X,
                direction.Y - 2 * dotProductResult * closestNormal.Y);

            var newDirection = new SPoint();
            newDirection.SetCoords(intersection.X + (reflectionPoint.X * directionRayOffset), intersection.Y + (reflectionPoint.Y * directionRayOffset));

            nextRay.Setup(newDirection, swapDensity);

            return nextRay;
        }

        
        private static NextRay ProcessSemiRough(Index1D index, ArrayView1D<int, Stride1D.Dense> seedArray, float directionRayOffset, SPoint closestNormal, SPoint raySource, SPoint intersection, float reflectionArea, bool swapDensity)
        {
            var nextRay = new NextRay();
            nextRay = ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity);

            var pointInCircle = PTGI_Random.GetPointInRadius(index, seedArray, reflectionArea);
            nextRay.Destination.Add(pointInCircle);

            return nextRay;
        }

        public static void IsRayStartingInPolygon(Point raySource, Polygon[] collisionObjects, ref Color pixel)
        {
            for (var i = 0; i < collisionObjects.Length; i++)
            {
                if (collisionObjects[i].reflectivnessType == PTGI_MaterialReflectivness.Transparent)
                    continue;
                
                var isInsideObject = raySource.LiesInObject(collisionObjects[i]);
                if (!isInsideObject) continue;
                
                if (collisionObjects[i].objectType == PTGI_ObjectTypes.LightSource)
                {
                    pixel.SetColor(collisionObjects[i].Color, collisionObjects[i].EmissionStrength);
                    pixel.Rescale(255);
                    pixel.ApplyGammaCorrection(1);
                    pixel.Clip();
                }
                pixel.Skip = 1;
                break;
            }
        }
    }
}
