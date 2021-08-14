using ILGPU;
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
using ILGPU.Algorithms;
using Grid = PTGI_Remastered.Structs.Grid;

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

        public RenderResult PathTraceRender(RenderSpecification renderSpecification)
        {
            Stopwatch processTimeStopwatch = new Stopwatch();
            processTimeStopwatch.Start();
            var walls = renderSpecification.GetWalls();
            var renderResult = new RenderResult();
            var bitmap = new Bitmap();
            bitmap.GridSize = renderSpecification.GridSize;
            bitmap.SetBitmapSettings(renderSpecification.ImageWidth, renderSpecification.ImageHeight, walls.Length);

            _cache.WithEnclosureDetection(bitmap, renderSpecification);
            _cache.WithContext();
            _cache.WithAccelerator(renderSpecification.GpuId, renderSpecification.UseCUDARenderer);
            _cache.SetPixelBuffer();
            _cache.SetSeedBuffer(bitmap.Size);
            _cache.SetWallBuffer(walls);
            _cache.SetGridDataBuffer(walls, bitmap, renderSpecification.GridSize);
            _cache.Finalize();

            var renderTimeStopwatch = new Stopwatch();
            renderTimeStopwatch.Start();
            renderResult.Pixels = StartRender(
                _cache.Accelerator, 
                bitmap, 
                _cache.PixelBuffer, 
                _cache.SeedBuffer, 
                _cache.WallBuffer, 
                renderSpecification.SampleCount, 
                renderSpecification.BounceLimit,
                _cache.GridDataBuffer,
                _cache.GridCached);
            renderTimeStopwatch.Stop();

            renderResult.RenderTime = renderTimeStopwatch.ElapsedMilliseconds;
            renderResult.bitmap = bitmap;
            processTimeStopwatch.Stop();
            renderResult.ProcessTime = processTimeStopwatch.ElapsedMilliseconds;
            return renderResult;
        }

        private static Color[] StartRender(Accelerator accelerator, Bitmap bitmap, MemoryBuffer<Color> bitmapPixels, MemoryBuffer<int> randomSeedArray, MemoryBuffer<Line> walls, int samples, int bounceLimit, MemoryBuffer3D<int> gridData, Grid grid)
        {
            var ptgiKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1,
                Bitmap,
                ArrayView<Color>,
                ArrayView<int>,
                int, 
                int, 
                ArrayView<Line>,
                ArrayView3D<int>,
                Grid>(CUDA_StartRender);

            try
            {
                ptgiKernel(bitmapPixels.Length, bitmap, bitmapPixels.View, randomSeedArray.View, samples, bounceLimit, walls.View, gridData.View, grid);
                accelerator.Synchronize();
                return bitmapPixels.GetAsArray();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            
            return new Color[bitmapPixels.Length];
        }

        private static void CUDA_StartRender(Index1 index, Bitmap bitmap, ArrayView<Color> pixels, ArrayView<int> seedArray, int samples, int bounceLimit, ArrayView<Line> walls, ArrayView3D<int> gridData, Grid grid)
        {
            if (index < pixels.Length)
            {
                if (pixels[index].Skip == 1)
                    return;
                
                Point raySource = PTGI_Math.Convert1dIndexTo2d(bitmap, index);
                Line lightRay = new Line();
                lightRay.Setup(raySource, new Point());
                var pixelInformaton = RenderBitmap(index, bitmap, lightRay, seedArray, samples, bounceLimit, walls, gridData, grid);

                bitmap.SetPixel(index, pixelInformaton.pixelColor, 1.0f / samples, pixels);
            }
        }

        private static PixelInformaton RenderBitmap(Index1 index, Bitmap bitmap, Line lightRay, ArrayView<int> seedArray, int samples, int bounceLimit, ArrayView<Line> walls, ArrayView3D<int> gridData, Grid grid)
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
                var rayTraceResult = CUDA_TraceRay(index, seedArray, bitmap, bounceLimit, false, walls, lightRay, wallToIgnore, gridData, grid);

                pixelInformation.pixelColor.Add(rayTraceResult.pixelColor);
            }
            return pixelInformation;
        }

        private static RayTraceResult CUDA_TraceRay(Index1 index, ArrayView<int> seedArray, Bitmap bitmap, int bounceLimit, bool originDensitySwap,
            ArrayView<Line> walls, Line lightRay, Line wallToIgnore, ArrayView3D<int> gridData, Grid grid)
        {
            var rayTraceResult = new RayTraceResult();
            rayTraceResult.pixelColor = new Color();
            rayTraceResult.pixelColor.SetColor(1, 1, 1);

            float reflectionAreaSize = 10;
            for (int bounceIndex = 0; bounceIndex < bounceLimit; bounceIndex++)
            {
                var gridTraversalResult = grid.TraverseGrid(lightRay, bitmap.WallsCount, gridData, walls, wallToIgnore);
                if (gridTraversalResult.IntesectedWall.HasValue != 1)
                {
                    rayTraceResult.pixelColor.SetColor(0, 0, 0);
                    return rayTraceResult;
                }
                else if(gridTraversalResult.IsClosestIntersectionLight)
                {
                    rayTraceResult.pixelColor.TintWith(gridTraversalResult.IntesectedWall.Color, gridTraversalResult.IntesectedWall.EmissionStrength);
                    rayTraceResult.pixelColor.Rescale(255);
                    return rayTraceResult;
                }
                else
                {
                    if(lightRay.Source.IsEqualTo(gridTraversalResult.IntersectionPoint))
                    {
                        rayTraceResult.pixelColor.SetColor(0, 0, 0);
                        return rayTraceResult;
                    }
                    wallToIgnore = gridTraversalResult.IntesectedWall;

                    var nextRayDirection = TraceRayUtility.NextRayDirection(index, seedArray, gridTraversalResult.IntesectedWall, wallToIgnore, lightRay.Source, gridTraversalResult.IntersectionPoint, reflectionAreaSize, originDensitySwap);
                    originDensitySwap = nextRayDirection.SwapDensity;

                    if(gridTraversalResult.IntesectedWall.ReflectivnessType == 4)
                    {
                        rayTraceResult.pixelColor.Multiply(gridTraversalResult.IntesectedWall.Color.GetRescaled(255));
                    }
                    else
                    {
                        rayTraceResult.pixelColor.Multiply(gridTraversalResult.IntesectedWall.Color.GetRescaled(255).GetRescaled(XMath.PI));
                    }

                    lightRay.Source = gridTraversalResult.IntersectionPoint;
                    lightRay.Destination = nextRayDirection.Destination;
                }
            }

            rayTraceResult.pixelColor.SetColor(0, 0, 0);
            return rayTraceResult;
        }
    }
}
