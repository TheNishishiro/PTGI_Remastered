using Alea;
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
        [GpuManaged]
        public static NextRay NextRayDirection(ref int randomSeed, Polygon collisionObject, Line wallToIgnore, Point raySource, Point intersection, double reflectionArea, bool swapDensity)
        {
            double reflectFromGlassChanceThreshold = 0.15;
            double directionRayOffset = 1000;
            double airDensity = 1;
            NextRay nextRay = new NextRay();

            var closestNormal = wallToIgnore.GetShiftedClosestNormal(raySource, intersection, reflectionArea);
            double reflectFromGlassChance = PTGI_Random.GetRandomBetween(ref randomSeed, 0, 1);

            var isReflectiveOrReflectFromGlass = collisionObject.reflectivnessType != PTGI_MaterialReflectivness.Transparent || reflectFromGlassChance < reflectFromGlassChanceThreshold;
            if (isReflectiveOrReflectFromGlass)
            {
                if (collisionObject.reflectivnessType == PTGI_MaterialReflectivness.Mirror)
                    nextRay = ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity);
                else if (collisionObject.reflectivnessType == PTGI_MaterialReflectivness.Reflective)
                    nextRay = ProcessSemiRough(ref randomSeed, directionRayOffset, closestNormal, raySource, intersection, reflectionArea, swapDensity);
                else
                    nextRay = ProcessRough(ref randomSeed, directionRayOffset, closestNormal, intersection, swapDensity);
            }
            else if(collisionObject.reflectivnessType == PTGI_MaterialReflectivness.Transparent)
            {
                nextRay = ProcessGlass(collisionObject, directionRayOffset, airDensity, closestNormal, raySource, intersection, swapDensity);
            }

            return nextRay;
        }

        [GpuManaged]
        private static NextRay ProcessRough(ref int randomSeed, double directionRayOffset, Point closestNormal, Point intersection, bool swapDensity)
        {
            NextRay nextRay = new NextRay();

            var newRayDirection = PTGI_Random.GetPointInRadius(ref randomSeed, closestNormal.GetDistance(intersection));
            newRayDirection.Add(closestNormal);

            var direction = intersection.GetDirection(newRayDirection);
            direction.Normalize();
            direction.Multiply(directionRayOffset);

            newRayDirection.Add(direction);

            nextRay.Setup(newRayDirection, swapDensity);

            return nextRay;
        }

        [GpuManaged]
        private static NextRay ProcessSmooth(Point closestNormal, double directionRayOffset, Point raySource, Point intersection, bool swapDensity)
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

        [GpuManaged]
        private static NextRay ProcessSemiRough(ref int randomSeed, double directionRayOffset, Point closestNormal, Point raySource, Point intersection, double reflectionArea, bool swapDensity)
        {
            NextRay nextRay = new NextRay();
            nextRay = ProcessSmooth(closestNormal, directionRayOffset, raySource, intersection, swapDensity);

            var PointInCircle = PTGI_Random.GetPointInRadius(ref randomSeed, reflectionArea);
            nextRay.Destination.Add(PointInCircle);

            return nextRay;
        }

        [GpuManaged]
        private static NextRay ProcessGlass(Polygon collisionObject, double directionRayOffset, double airDensity, Point closestNormal, Point raySource, Point intersection, bool swapDensity)
        {
            NextRay nextRay = new NextRay();

            double mi1, mi2;
            mi1 = mi2 = airDensity;
            if (!swapDensity)
                mi2 = collisionObject.Density;
            else
                mi1 = collisionObject.Density;

            Point direction = raySource.GetDirection(intersection);
            direction.Normalize();

            var invertedNormal = closestNormal.MultiplyNew(-1);

            double r = mi1 / mi2;
            double c = invertedNormal.DotProduct(direction);
            double critical = (r * r) * (1 - c * c);

            if (critical <= 1)
            {
                nextRay = ReflectInternally(nextRay, r, c, directionRayOffset, direction, intersection, closestNormal, swapDensity);
            }
            else
            {
                nextRay = PassThrough(nextRay, raySource, intersection, closestNormal, directionRayOffset, swapDensity);
            }

            return nextRay;
        }

        [GpuManaged]
        private static NextRay ReflectInternally(NextRay nextRay, double r, double c, double directionRayOffset, Point direction, Point intersection, Point normal, bool swapDensity)
        {
            Point destination = new Point();
            var factor = (r * c - DeviceFunction.Sqrt(1 - (r * r) * (1 - c * c)));

            destination.SetCoords(
                r * direction.X + factor * normal.X,
                r * direction.Y + factor * normal.Y);

            Point rayDestination = destination.MultiplyNew(directionRayOffset).AddNew(intersection);

            nextRay.Setup(rayDestination, swapDensity);
            return nextRay;
        }

        [GpuManaged]
        private static NextRay PassThrough(NextRay nextRay, Point raySource, Point intersection, Point closestNormal, double directionRayOffset, bool swapDensity)
        {
            swapDensity = !swapDensity;

            var direction = raySource.GetDirection(intersection);
            direction.Normalize();

            Point destination = new Point();
            var dotProduct = direction.DotProduct(closestNormal);

            destination.SetCoords(
                direction.X - 2 * dotProduct * closestNormal.X,
                direction.Y - 2 * dotProduct * closestNormal.Y);

            Point rayDestination = destination.MultiplyNew(directionRayOffset).AddNew(intersection);
            nextRay.Setup(rayDestination, swapDensity);
            return nextRay;
        }

        /// <summary>
        /// Checks if point specified is contained inside of polygon and get it's color data
        /// </summary>
        /// <param name="raySource">Point to check for</param>
        /// <param name="collisionObjects">Array of polygons to check with</param>
        /// <returns>PixelInformaton.ShouldCancelRender if object lies within non-transparent polygon and it's color data</returns>
        [GpuManaged]
        public static PixelInformaton IsRayStartingInPolygon(Point raySource, Polygon[] collisionObjects)
        {
            PixelInformaton pixelInformaton = new PixelInformaton();
            pixelInformaton.DensityRegionSwap = false;
            pixelInformaton.pixelColor = new Color();
            for (int i = 0; i < collisionObjects.Length; i++)
            {
                bool isInsideObject = raySource.LiesInObject(collisionObjects[i]);
                if (isInsideObject)
                {
                    if (collisionObjects[i].reflectivnessType == PTGI_MaterialReflectivness.Transparent)
                        pixelInformaton.DensityRegionSwap = true;
                    else
                    {
                        if (collisionObjects[i].objectType == PTGI_ObjectTypes.LightSource)
                        {
                            pixelInformaton.pixelColor.SetColor(collisionObjects[i].Color, collisionObjects[i].EmissionStrength);
                            pixelInformaton.pixelColor.Rescale(255);
                        }
                        pixelInformaton.ShouldCancelRender = true;
                        break;
                    }
                }
            }
            return pixelInformaton;
        }

        
    }
}
