using ILGPU;
using ILGPU.Algorithms;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Utilities
{
    public struct NextRay
    {
        public Point Destination;
        public bool SwapDensity;
        public bool HasValue;

        public void Setup(Point Destination, bool SwapDensity)
        {
            this.Destination = Destination;
            this.SwapDensity = SwapDensity;
            HasValue = true;
        }
    }

    public class TraceRayUtility
    {
        
        public static NextRay NextRayDirection(Index1 index, ArrayView<int> seedArray, Line collisionObject, Line wallToIgnore, Point raySource, Point intersection, float reflectionArea, bool swapDensity)
        {
            float reflectFromGlassChanceThreshold = 0.15f;
            float directionRayOffset = 1000;
            float airDensity = 1;
            NextRay nextRay = new NextRay();

            var closestNormal = wallToIgnore.GetShiftedClosestNormal(raySource, intersection, reflectionArea);
            float reflectFromGlassChance = PTGI_Random.GetRandomBetween(index, seedArray, 0, 1);

            var isReflectiveOrReflectFromGlass = collisionObject.ReflectivnessType != 4 || reflectFromGlassChance < reflectFromGlassChanceThreshold;
            if (isReflectiveOrReflectFromGlass)
            {
                if (collisionObject.ReflectivnessType == 3)
                    nextRay = ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity);
                else if (collisionObject.ReflectivnessType == 2)
                    nextRay = ProcessSemiRough(index, seedArray, directionRayOffset, closestNormal, raySource, intersection, reflectionArea, swapDensity);
                else
                    nextRay = ProcessRough(index, seedArray, directionRayOffset, closestNormal, intersection, swapDensity);
            }

            return nextRay;
        }

        
        private static NextRay ProcessRough(Index1 index, ArrayView<int> seedArray, float directionRayOffset, Point closestNormal, Point intersection, bool swapDensity)
        {
            NextRay nextRay = new NextRay();

            var newRayDirection = PTGI_Random.GetPointInRadius(index, seedArray, closestNormal.GetDistance(intersection));
            newRayDirection.Add(closestNormal);

            var direction = intersection.GetDirection(newRayDirection);
            direction.Normalize();
            direction.Multiply(directionRayOffset);

            newRayDirection.Add(direction);

            nextRay.Setup(newRayDirection, swapDensity);

            return nextRay;
        }

        
        private static NextRay ProcessSmooth(Point closestNormal, float directionRayOffset, Point raySource, Point intersection, bool swapDensity)
        {
            NextRay nextRay = new NextRay();

            var direction = raySource.GetDirection(intersection);
            direction.Normalize();

            Point reflectionPoint = new Point();
            var dotProductResult = direction.DotProduct(closestNormal);
            reflectionPoint.SetCoords(
                direction.X - 2 * dotProductResult * closestNormal.X,
                direction.Y - 2 * dotProductResult * closestNormal.Y);

            Point newDirection = new Point();
            newDirection.SetCoords(intersection.X + (reflectionPoint.X * directionRayOffset), intersection.Y + (reflectionPoint.Y * directionRayOffset));

            nextRay.Setup(newDirection, swapDensity);

            return nextRay;
        }

        
        private static NextRay ProcessSemiRough(Index1 index, ArrayView<int> seedArray, float directionRayOffset, Point closestNormal, Point raySource, Point intersection, float reflectionArea, bool swapDensity)
        {
            NextRay nextRay = new NextRay();
            nextRay = ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity);

            var PointInCircle = PTGI_Random.GetPointInRadius(index, seedArray, reflectionArea);
            nextRay.Destination.Add(PointInCircle);

            return nextRay;
        }
    }
}
