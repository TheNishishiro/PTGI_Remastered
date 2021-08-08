using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
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
        public List<Gpu> GetAvaiableHardwareAccelerators()
        {
            var context = new Context();
            var gpus = new List<Gpu>();
            foreach (var acceleratorId in Accelerator.Accelerators)
            {
                using (var accl = Accelerator.Create(context, acceleratorId))
                {
                    gpus.Add(new Gpu()
                    {
                        Id = acceleratorId,
                        Name = accl.Name
                    });

                    accl.PrintInformation();
                }
            }
            context.Dispose();

            return gpus;
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
        public RenderResult PathTraceRender(Polygon[] collisionObjects, int imageWidth, int imageHeight, int samples, int bounceLimit, int gridDivides, bool UseCUDARenderer, AcceleratorId GpuId)
        {
            var polygons = new List<List<Line>>();
            int index = 0;
            foreach (var polygon in collisionObjects)
            {
                polygons.Add(new List<Line>());
                foreach (var wall in polygon.Walls)
                {
                    polygons[index].Add(new Line()
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
                    });
                }
                index++;
            }

            var context = new Context(ContextFlags.Force32BitFloats, ILGPU.IR.Transformations.OptimizationLevel.O2);
            Accelerator accelerator = null;

            if (GpuId != null)
                accelerator = Accelerator.Create(context, GpuId);
            else if (UseCUDARenderer)
                accelerator = new CudaAccelerator(context);
            else
                accelerator = new CPUAccelerator(context);

            RenderResult renderResult = new RenderResult();
            Bitmap bitmap = new Bitmap();
            var bitmapPixels = bitmap.CreateColorArrayView(imageWidth, imageHeight, walls.Count, accelerator);

            Random rnd = new Random();
            int[] randomSeed = new int[bitmap.Size];
            Parallel.For(0, bitmap.Size, (i) =>
            {
                randomSeed[i] = PTGI_Random.Next();
            });
            var seedArrayView = accelerator.Allocate<int>(randomSeed);
            var wallArrayView = accelerator.Allocate<Line>(polygons.Select(a => a.ToArray()).ToArray(),);

            Stopwatch renderTimeStopwatch = new Stopwatch();
            renderTimeStopwatch.Start();
            renderResult.Pixels = StartRender(accelerator, bitmap, bitmapPixels, seedArrayView, wallArrayView, samples, bounceLimit);
            renderTimeStopwatch.Stop();

            var isAnyPixelColored = renderResult.Pixels.Any(x => x.B != 0 || x.G != 0 || x.R != 0);

            renderResult.RenderTime = renderTimeStopwatch.ElapsedMilliseconds;
            renderResult.bitmap = bitmap;

            context.Dispose();
            wallArrayView.Dispose();
            seedArrayView.Dispose();
            bitmapPixels.Dispose();
            accelerator.ClearCache(ClearCacheMode.Default);
            accelerator.Dispose();
            return renderResult;
        }

        private static Color[] StartRender(Accelerator accelerator, Bitmap bitmap, MemoryBuffer<Color> bitmapPixels, MemoryBuffer<int> randomSeedArray, MemoryBuffer<Line> walls, int samples, int bounceLimit)
        {
            
            var ptgiKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1,
                Bitmap,
                ArrayView<Color>,
                ArrayView<int>,
                int, int, ArrayView<Line>>(CUDA_StartRender);

            try
            {
                ptgiKernel(bitmapPixels.Length, bitmap, bitmapPixels.View, randomSeedArray.View, samples, bounceLimit, walls.View);
                accelerator.Synchronize();
                return bitmapPixels.GetAsArray();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            
            return null;
        }

        private static void CUDA_StartRender(Index1 index, Bitmap bitmap, ArrayView<Color> pixels, ArrayView<int> seedArray, int samples, int bounceLimit, ArrayView<Line> walls)
        {
            int CUDAThreadId = index;
            if (CUDAThreadId < pixels.Length)
            {
                Point raySource = PTGI_Math.GetRaySourceFromThreadIndex(bitmap, CUDAThreadId);
                Line lightRay = new Line();
                lightRay.Setup(raySource, new Point());
                var pixelInformaton = RenderBitmap(index, bitmap, lightRay, seedArray, samples, bounceLimit, walls);
                //if (pixelInformaton.pixelColor.R != 0 || pixelInformaton.pixelColor.G != 0|| pixelInformaton.pixelColor.B!=0)
                //    Interop.WriteLine("Color {0}, {1}, {2}", pixelInformaton.pixelColor.R, pixelInformaton.pixelColor.G, pixelInformaton.pixelColor.B);

                bitmap.SetPixel(CUDAThreadId, pixelInformaton.pixelColor, 1.0f / samples, pixels);
            }
        }

        private static PixelInformaton RenderBitmap(Index1 index, Bitmap bitmap, Line lightRay, ArrayView<int> seedArray, int samples, int bounceLimit, ArrayView<Line> walls)
        {
            var pixelInformation = new PixelInformaton(); 
            for (int sampleId = 0; sampleId < samples; sampleId++)
            {
                lightRay.Destination = PTGI_Random.GetPointInRadius(index, seedArray, 10).AddNew(lightRay.Source);

                Point rayDirection = lightRay.GetDirection();
                rayDirection.Normalize();
                rayDirection.Multiply(10000);

                lightRay.Destination.Add(rayDirection);
                Line wallToIgnore = new Line();
                var rayTraceResult = CUDA_TraceRay(index, seedArray, bitmap, bounceLimit, false, walls, lightRay, wallToIgnore);

                pixelInformation.pixelColor.Add(rayTraceResult.pixelColor);
            }
            return pixelInformation;
        }

        private static RayTraceResult CUDA_TraceRay(Index1 index, ArrayView<int> seedArray, Bitmap bitmap, int bounceLimit, bool originDensitySwap,
            ArrayView<Line> walls, Line lightRay, Line wallToIgnore)
        {
            var rayTraceResult = new RayTraceResult();
            rayTraceResult.pixelColor = new Color();
            rayTraceResult.pixelColor.SetColor(1, 1, 1);

            float reflectionAreaSize = 10;
            for (int bounceIndex = 0; bounceIndex < bounceLimit; bounceIndex++)
            {
                float closestDistance = float.MaxValue;
                Point intersectionPoint = new Point();
                bool isClosestIntersectionLight = false;
                var closestIntersectionObjectId = int.MaxValue;
                var wallCollidedWith = new Line();

                for(int i = 0; i < bitmap.WallsCount; i++)
                {
                    var rayWallIntersection = walls[i].GetIntersection(lightRay, wallToIgnore);
                    if (rayWallIntersection.HasValue == 1)
                    {
                        var raySourceToWallIntersectionDistance = lightRay.Source.GetDistance(rayWallIntersection);
                        if (raySourceToWallIntersectionDistance < closestDistance)
                        {
                            intersectionPoint = rayWallIntersection;
                            closestDistance = raySourceToWallIntersectionDistance;
                            isClosestIntersectionLight = walls[i].ObjectType == 2 ? true : false;
                            wallCollidedWith = walls[i];
                            wallCollidedWith.HasValue = 1;
                        }
                    }
                }

                if(wallCollidedWith.HasValue != 1)
                {
                    rayTraceResult.pixelColor.SetColor(0, 0, 0);
                    return rayTraceResult;
                }
                else if(isClosestIntersectionLight)
                {
                    rayTraceResult.pixelColor.TintWith(wallCollidedWith.Color, wallCollidedWith.EmissionStrength);
                    rayTraceResult.pixelColor.Rescale(255);
                    return rayTraceResult;
                }
                else
                {
                    if(lightRay.Source.IsEqualTo(intersectionPoint))
                    {
                        rayTraceResult.pixelColor.SetColor(0, 0, 0);
                        return rayTraceResult;
                    }
                    wallToIgnore = wallCollidedWith;

                    var nextRayDirection = TraceRayUtility.NextRayDirection(index, seedArray, wallCollidedWith, wallToIgnore, lightRay.Source, intersectionPoint, reflectionAreaSize, originDensitySwap);
                    originDensitySwap = nextRayDirection.SwapDensity;

                    if(wallCollidedWith.ReflectivnessType == 4)
                    {
                        rayTraceResult.pixelColor.Multiply(wallCollidedWith.Color.GetRescaled(255));
                    }
                    else
                    {
                        rayTraceResult.pixelColor.Multiply(wallCollidedWith.Color.GetRescaled(255).GetRescaled(3.14159265359f));
                    }

                    lightRay.Source = intersectionPoint;
                    lightRay.Destination = nextRayDirection.Destination;
                }
            }

            rayTraceResult.pixelColor.SetColor(0, 0, 0);
            return rayTraceResult;
        }
    }
}
