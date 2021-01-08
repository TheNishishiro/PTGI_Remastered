using Alea;
using Alea.CSharp;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered
{
    public class PTGI
    {
        /// <summary>
        /// Iterates over CUDA capable GPUs installed on system
        /// </summary>
        /// <returns>List of available GPUs as PTGI_Gpu</returns>
        public List<PTGI_Remastered.Structs.Gpu> GetAvailableGpus()
        {
            List<PTGI_Remastered.Structs.Gpu> myGpus = new List<Structs.Gpu>();
            try
            {
                var gpus = Device.Devices;
                foreach (var gpu in gpus)
                {
                    myGpus.Add(new Structs.Gpu() { Id = gpu.Id, Name = gpu.Name });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return myGpus;
        }

        public RayTraceResult DebugRay(Polygon[] collisionObjects, Point source, Point Destination, int imageWidth, int imageHeight, int bounceLimit, int gridDivides)
        {
            List<Point> rayTrace = new List<Point>();
            Bitmap bitmap = new Bitmap();
            bitmap.Create(imageWidth, imageHeight);

            Random rnd = new Random();
            Grid cellGrid = new Grid();
            cellGrid.Create(bitmap, gridDivides);
            cellGrid.CPU_FillGrid(collisionObjects);

            Line ray = new Line();
            ray.Setup(source, Destination);

            int[] randomSeedArray = new int[bitmap.Size];
            int randomSeed = rnd.Next(1000000);
            RayTraceResult rayTraceResult = new RayTraceResult();
            rayTraceResult.collectRayTrace = true;
            rayTraceResult.rayTrace = new Point[bounceLimit+1];
            rayTraceResult.rayGridMovement = new int[gridDivides * 4];
            for (int i = 0; i < rayTraceResult.rayGridMovement.Length; i++)
                rayTraceResult.rayGridMovement[i] = int.MaxValue;

            var traceResult = CUDA_TraceRay(ref randomSeed, bounceLimit, false, ref cellGrid, collisionObjects, ray, new Line(), rayTraceResult);

            return traceResult;
        }

        /// <summary>
        /// Prepares data and initiates path tracer 
        /// </summary>
        /// <param name="collisionObjects">World objects that light interacts with</param>
        /// <param name="imageWidth">Render width</param>
        /// <param name="imageHeight">Render height</param>
        /// <param name="samples">Number of light samples per pixel</param>
        /// <param name="bounceLimit">Max number of light bounces</param>
        /// <param name="gridDivides">Number of columns</param>
        /// <param name="UseCUDARenderer">Specified to use GPU for rendering</param>
        /// <param name="GpuId">GPU to use for rendering</param>
        /// <returns>Rendered result as RenderResult</returns>
        public RenderResult PathTraceRender(Polygon[] collisionObjects, int imageWidth, int imageHeight, int samples, int bounceLimit, int gridDivides, bool UseCUDARenderer, int GpuId)
        {
            RenderResult renderResult = new RenderResult();
            Bitmap bitmap = new Bitmap();
            bitmap.Create(imageWidth, imageHeight);

            Random rnd = new Random();
            Grid cellGrid = new Grid();
            cellGrid.Create(bitmap, gridDivides);
            cellGrid.CPU_FillGrid(collisionObjects);

            int[] randomSeedArray = new int[bitmap.Size];
            Parallel.For(0, bitmap.Size, (i) =>
            {
                randomSeedArray[i] = PTGI_Random.Next();
            });

            Stopwatch renderTimeStopwatch = new Stopwatch();
            renderTimeStopwatch.Start();
            StartRender(bitmap, cellGrid, randomSeedArray, collisionObjects, samples, bounceLimit, UseCUDARenderer, GpuId);
            renderTimeStopwatch.Stop();

            renderResult.RenderTime = renderTimeStopwatch.ElapsedMilliseconds;
            renderResult.bitmap = bitmap;

            return renderResult;
        }

        private static void StartRender(Bitmap bitmap, Grid cellGrid, int[] randomSeedArray, Polygon[] collisionObjects, int samples, int bounceLimit, bool UseCUDARenderer, int GpuId)
        {
            PolygonContainer polygonContainer = new PolygonContainer();
            polygonContainer.PolygonCount = collisionObjects.Length;
            Line[][] walls = new Line[collisionObjects.Length][];
            Point[][] verticies = new Point[collisionObjects.Length][];

            polygonContainer.Setup(collisionObjects, walls, verticies);

            if (UseCUDARenderer)
            {
                Alea.Gpu gpu = Alea.Gpu.Get(GpuId);
                int block_size = 128;
                int grid_size = (((bitmap.Size) + block_size) / block_size);
                var lp = new LaunchParam(grid_size, block_size);

                try
                {
                    gpu.Launch(CUDA_StartRender, lp, bitmap.Width, bitmap.Height, bitmap.pixels, cellGrid, randomSeedArray, samples, bounceLimit, polygonContainer, walls, verticies);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            else
            {
                for(int i = 0; i < bitmap.Size; i++)
                    CPU_StartRender(i, bitmap.Width, bitmap.Height, bitmap.pixels, cellGrid, randomSeedArray, samples, bounceLimit, polygonContainer, walls, verticies);

                //Parallel.For(0, bitmap.Size, (i) =>
                //{
                //    CPU_StartRender(i, bitmap.Width, bitmap.Height, bitmap.pixels, cellGrid, randomSeedArray, samples, bounceLimit, polygonContainer, walls, verticies);
                //});
            }
        }

        private static void CPU_StartRender(int ThreadId, int bitmapWidth, int bitmapHeight, Color[] pixels, Grid cellGrid, int[] seedArray, int samples, int bounceLimit, PolygonContainer polygonContainer, Line[][] walls, Point[][] verticies)
        {

            if (ThreadId < pixels.Length)
            {
                Bitmap bitmap = new Bitmap();
                bitmap.CopyPixels(pixels, bitmapWidth, bitmapHeight);

                Polygon[] CUDA_collisionObjects = new Polygon[polygonContainer.PolygonCount];
                polygonContainer.CUDA_Copy(CUDA_collisionObjects, walls, verticies);

                Point raySource = PTGI_Math.GetRaySourceFromThreadIndex(ref bitmap, ThreadId);

                Line lightRay = new Line();
                lightRay.Setup(raySource, new Point());

                var pixelInformaton = TraceRayUtility.IsRayStartingInPolygon(raySource, CUDA_collisionObjects);

                if (pixelInformaton.ShouldCancelRender)
                {
                    bitmap.SetPixel(ThreadId, pixelInformaton.pixelColor, 1);
                    return;
                }
                pixelInformaton = RenderBitmap(ref bitmap, pixelInformaton, lightRay, ref cellGrid, ref seedArray[ThreadId], samples, bounceLimit, CUDA_collisionObjects);
                bitmap.SetPixel(ThreadId, pixelInformaton.pixelColor, 1.0 / (double)samples);
            }
        }

        [GpuManaged]
        private static void CUDA_StartRender(int bitmapWidth, int bitmapHeight, Color[] pixels, Grid cellGrid, int[] seedArray, int samples, int bounceLimit, PolygonContainer polygonContainer, Line[][] walls, Point[][] verticies)
        {
            int CUDAThreadId = blockIdx.x * blockDim.x + threadIdx.x;

            if (CUDAThreadId < pixels.Length)
            {
                Bitmap bitmap = new Bitmap();
                bitmap.CopyPixels(pixels, bitmapWidth, bitmapHeight);

                Polygon[] CUDA_collisionObjects = __local__.Array<Polygon>(1000);
                polygonContainer.CUDA_Copy(CUDA_collisionObjects, walls, verticies);
                
                Point raySource = PTGI_Math.GetRaySourceFromThreadIndex(ref bitmap, CUDAThreadId);

                Line lightRay = new Line();
                lightRay.Setup(raySource, new Point());

                var pixelInformaton = TraceRayUtility.IsRayStartingInPolygon(raySource, CUDA_collisionObjects);
                
                if (pixelInformaton.ShouldCancelRender)
                {
                    bitmap.SetPixel(CUDAThreadId, pixelInformaton.pixelColor, 1);
                    return;
                }
                pixelInformaton = RenderBitmap(ref bitmap, pixelInformaton, lightRay, ref cellGrid, ref seedArray[CUDAThreadId], samples, bounceLimit, CUDA_collisionObjects);
                bitmap.SetPixel(CUDAThreadId, pixelInformaton.pixelColor, 1.0 / (double)samples);
            }
        }

        [GpuManaged]
        private static PixelInformaton RenderBitmap(ref Bitmap bitmap, PixelInformaton pixelInformaton, Line lightRay, ref Grid cellGrid, ref int randomSeed, int samples, int bounceLimit, Polygon[] collisionObjects)
        {
            for(int sampleId = 0; sampleId < samples; sampleId++)
            {
                lightRay.Destination = PTGI_Random.GetPointInRadius(ref randomSeed, 10).AddNew(lightRay.Source);

                Point rayDirection = lightRay.GetDirection();
                rayDirection.Normalize();
                rayDirection.Multiply(10000);

                lightRay.Destination.Add(rayDirection);
                Line wallToIgnore = new Line();
                RayTraceResult rayTraceResult = new RayTraceResult();
                rayTraceResult = CUDA_TraceRay(ref randomSeed, bounceLimit, pixelInformaton.DensityRegionSwap, ref cellGrid, collisionObjects, lightRay, wallToIgnore, rayTraceResult);

                pixelInformaton.pixelColor.Add(rayTraceResult.pixelColor);
            }
            return pixelInformaton;
        }

        [GpuManaged]
        private static RayTraceResult CUDA_TraceRay(ref int randomSeed, int bounceLimit, bool originDensitySwap,
            ref Grid cellGrid, Polygon[] collisionObjects, Line lightRay, Line wallToIgnore, RayTraceResult rayTraceResult)
        {
            rayTraceResult.pixelColor = new Color();
            rayTraceResult.pixelColor.SetColor(1, 1, 1);

            double reflectionAreaSize = 10;
            
            for(int bounceIndex = 0; bounceIndex < bounceLimit; bounceIndex++)
            {
                rayTraceResult.AddRayDebugPoint(lightRay.Source);

                for (int i = 0; i < collisionObjects.Length; i++)
                {
                    collisionObjects[i].ResetWallCheckedStatus();
                }

                double closestDistance = double.MaxValue;
                Point intersectionPoint = new Point();
                bool isClosestIntersectionLight = false;
                var closestIntersectionObjectId = int.MaxValue;
                var gridVariables = cellGrid.CUDAGetGridTraversalVariables(lightRay);
                var objectCollidedWith = new Polygon();
                var wallCollidedWith = new Line();

                while(true)
                {
                    int currentGridCellIndex = cellGrid.CUDAGetCellIndex(gridVariables);
                    if (cellGrid.CUDAIsOutsideGrid(gridVariables))
                        break;

                    rayTraceResult.AddGridDebugPoint(currentGridCellIndex);
                    var cellObjects = cellGrid.GetObjectsInCell(currentGridCellIndex);

                    for(int cellObjectId = 0; cellObjectId < cellObjects.Length; cellObjectId++)
                    {
                        var collisionObjectIds = cellObjects[cellObjectId];

                        var polygonObject = collisionObjects[collisionObjectIds.PolygonId];
                        var rayWallIntersection = polygonObject.Walls[collisionObjectIds.WallId].GetIntersection(lightRay, wallToIgnore);

                        if (rayWallIntersection.HasValue)
                        {
                            var raySourceToWallIntersectionDistance = lightRay.Source.GetDistance(rayWallIntersection);
                            if (raySourceToWallIntersectionDistance < closestDistance)
                            {
                                intersectionPoint = rayWallIntersection;
                                closestDistance = raySourceToWallIntersectionDistance;
                                isClosestIntersectionLight = polygonObject.objectType == PTGI_ObjectTypes.LightSource ? true : false;
                                closestIntersectionObjectId = collisionObjectIds.PolygonId;
                                wallCollidedWith = polygonObject.Walls[collisionObjectIds.WallId];
                                wallCollidedWith.HasValue = true;
                                objectCollidedWith = collisionObjects[closestIntersectionObjectId];
                            }
                        }
                    }

                    if(intersectionPoint.HasValue)
                        break;

                    gridVariables.TraverseToNextCell();
                }

                if(!objectCollidedWith.HasValue)
                {
                    rayTraceResult.pixelColor.SetColor(0, 0, 0);
                    rayTraceResult.AddRayDebugPoint(lightRay.Destination);
                    return rayTraceResult;
                }
                else if(isClosestIntersectionLight)
                {
                    rayTraceResult.pixelColor.TintWith(objectCollidedWith.Color, objectCollidedWith.EmissionStrength);
                    rayTraceResult.pixelColor.Rescale(255);
                    rayTraceResult.AddRayDebugPoint(intersectionPoint);
                    return rayTraceResult;
                }
                else if(!isClosestIntersectionLight)
                {
                    if(lightRay.Source.IsEqualTo(intersectionPoint))
                    {
                        rayTraceResult.pixelColor.SetColor(0, 0, 0);
                        rayTraceResult.AddRayDebugPoint(intersectionPoint);
                        return rayTraceResult;
                    }
                    wallToIgnore = wallCollidedWith;

                    var nextRayDirection = TraceRayUtility.NextRayDirection(ref randomSeed, objectCollidedWith, wallToIgnore, lightRay.Source, intersectionPoint, reflectionAreaSize, originDensitySwap);
                    originDensitySwap = nextRayDirection.SwapDensity;

                    if(objectCollidedWith.reflectivnessType == PTGI_MaterialReflectivness.Transparent)
                    {
                        rayTraceResult.pixelColor.Multiply(objectCollidedWith.Color.GetRescaled(255));
                    }
                    else
                    {
                        rayTraceResult.pixelColor.Multiply(objectCollidedWith.Color.GetRescaled(255).GetRescaled(3.14159265359));
                    }

                    lightRay.Source = intersectionPoint;
                    lightRay.Destination = nextRayDirection.Destination;
                }
            }

            rayTraceResult.pixelColor.SetColor(0, 0, 0);
            rayTraceResult.AddRayDebugPoint(lightRay.Source);
            return rayTraceResult;
        }
    }
}
