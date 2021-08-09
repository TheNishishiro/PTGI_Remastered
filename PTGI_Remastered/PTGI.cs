﻿using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTGI_Remastered.Cache;

namespace PTGI_Remastered
{
    public class PTGI
    {
        private TempCache _cache;

        public PTGI()
        {
            _cache = new TempCache();
        }
        
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
        public RenderResult PathTraceRender(RenderSpecification renderSpecification)
        {
            Stopwatch processTimeStopwatch = new Stopwatch();
            processTimeStopwatch.Start();
            var walls = renderSpecification.GetWalls();
            var renderResult = new RenderResult();
            var bitmap = new Bitmap();
            bitmap.SetBitmapSettings(renderSpecification.ImageWidth, renderSpecification.ImageHeight, walls.Length);
            var pixels = new Color[bitmap.Size];
            var randomSeed = new int[bitmap.Size];
            Parallel.For(0, bitmap.Size, (i) =>
            {
                if (renderSpecification.IgnoreEnclosedPixels)
                {
                    var point = PTGI_Math.GetRaySourceFromThreadIndex(bitmap, i);
                    TraceRayUtility.IsRayStartingInPolygon(point, renderSpecification.Objects, renderSpecification.SampleCount, ref pixels[i]);
                }
                randomSeed[i] = PTGI_Random.Next();
            });
            
            _cache.WithContext();
            _cache.WithAccelerator(renderSpecification.GpuId, renderSpecification.UseCUDARenderer);
            _cache.SetPixelBuffer(pixels);
            _cache.SetSeedBuffer(randomSeed);
            _cache.SetWallBuffer(walls);

            var renderTimeStopwatch = new Stopwatch();
            renderTimeStopwatch.Start();
            renderResult.Pixels = StartRender(_cache.Accelerator, bitmap, _cache.PixelBuffer, _cache.SeedBuffer, _cache.WallBuffer, renderSpecification.SampleCount, renderSpecification.BounceLimit);
            renderTimeStopwatch.Stop();

            renderResult.RenderTime = renderTimeStopwatch.ElapsedMilliseconds;
            renderResult.bitmap = bitmap;
            processTimeStopwatch.Stop();
            
            renderResult.ProcessTime = processTimeStopwatch.ElapsedMilliseconds;
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
            
            return new Color[bitmapPixels.Length];
        }

        private static void CUDA_StartRender(Index1 index, Bitmap bitmap, ArrayView<Color> pixels, ArrayView<int> seedArray, int samples, int bounceLimit, ArrayView<Line> walls)
        {
            if (index < pixels.Length)
            {
                if (pixels[index].Skip == 1)
                    return;
                
                Point raySource = PTGI_Math.GetRaySourceFromThreadIndex(bitmap, index);
                Line lightRay = new Line();
                lightRay.Setup(raySource, new Point());
                var pixelInformaton = RenderBitmap(index, bitmap, lightRay, seedArray, samples, bounceLimit, walls);

                bitmap.SetPixel(index, pixelInformaton.pixelColor, 1.0f / samples, pixels);
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
