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
using ILGPU.Runtime.OpenCL;

namespace PTGI_Remastered
{
    public class PTGI
    {
        private TempCache _cache;

        public PTGI()
        {
            _cache = new TempCache();
        }
        
        public List<Gpu> GetAvailableHardwareAccelerators()
        {
            var context = Context.CreateDefault();
            
            var gpus = new List<Gpu>();
            foreach (var device in context)
            {
                using var accelerator = device.CreateAccelerator(context);
                switch (accelerator)
                {
                    case CPUAccelerator cpuAccelerator:
                        gpus.Add(new Gpu(cpuAccelerator));
                        break;
                    case CudaAccelerator cudaAccelerator:
                        gpus.Add(new Gpu(cudaAccelerator));
                        break;
                    case CLAccelerator clAccelerator:
                        gpus.Add(new Gpu(clAccelerator));
                        break;
                }

                accelerator.PrintInformation();
            }
            context.Dispose();

            return gpus;
        }

        public RenderResult PathTraceRender(RenderSpecification renderSpecification)
        {
            var processTimeStopwatch = new Stopwatch();
            processTimeStopwatch.Start();
            var walls = renderSpecification.GetWalls();
            var renderResult = new RenderResult();
            var bitmap = new Bitmap();
            bitmap.GridSize = renderSpecification.GridSize;
            bitmap.SetBitmapSettings(renderSpecification.ImageWidth, renderSpecification.ImageHeight, walls.Length);

            _cache.WithEnclosureDetection(bitmap, renderSpecification);
            _cache.WithContext(renderSpecification.DeviceId, renderSpecification.AcceleratorType);
            _cache.SetPixelBuffer();
            _cache.SetSeedBuffer(bitmap.Size, renderSpecification.Seed);
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
                _cache.SGridCached);
            renderTimeStopwatch.Stop();

            renderResult.RenderTime = renderTimeStopwatch.ElapsedMilliseconds;
            renderResult.bitmap = bitmap;
            processTimeStopwatch.Stop();
            renderResult.ProcessTime = processTimeStopwatch.ElapsedMilliseconds;
            return renderResult;
        }

        private static Color[] StartRender(Accelerator accelerator, Bitmap bitmap, MemoryBuffer1D<Color, Stride1D.Dense> bitmapPixels, MemoryBuffer1D<int, Stride1D.Dense> randomSeedArray, MemoryBuffer1D<SLine, Stride1D.Dense> walls, int samples, int bounceLimit, MemoryBuffer1D<int, Stride1D.Dense> gridData, SGrid sGrid)
        {
            var ptgiKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                Bitmap,
                ArrayView1D<Color, Stride1D.Dense>,
                ArrayView1D<int, Stride1D.Dense>,
                int, 
                int, 
                ArrayView1D<SLine, Stride1D.Dense>,
                ArrayView1D<int, Stride1D.Dense>,
                SGrid>(CUDA_StartRender);

            try
            {
                ptgiKernel((int)bitmapPixels.Length, bitmap, bitmapPixels.View, randomSeedArray.View, samples, bounceLimit, walls.View, gridData.View, sGrid);
                accelerator.Synchronize();
                return bitmapPixels.AsContiguous().GetAsArray();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            
            return new Color[bitmapPixels.Length];
        }

        private static void CUDA_StartRender(Index1D index, Bitmap bitmap, ArrayView1D<Color, Stride1D.Dense> pixels, ArrayView1D<int, Stride1D.Dense> seedArray, int samples, int bounceLimit, ArrayView1D<SLine, Stride1D.Dense> walls, ArrayView1D<int, Stride1D.Dense> gridData, SGrid sGrid)
        {
            if (pixels[index].Skip == 1)
                return;
            
            SPoint raySource = PTGI_Math.Convert1dIndexTo2d(bitmap, index);
            SLine lightRay = new SLine();
            lightRay.Setup(raySource, new SPoint());
            var pixelInformaton = RenderBitmap(index, bitmap, lightRay, seedArray, samples, bounceLimit, walls, gridData, sGrid);

            bitmap.SetPixel(index, pixelInformaton.pixelColor, 1.0f / samples, pixels);
        }

        private static PixelInformaton RenderBitmap(Index1D index, Bitmap bitmap, SLine lightRay, ArrayView1D<int, Stride1D.Dense> seedArray, int samples, int bounceLimit, ArrayView1D<SLine, Stride1D.Dense> walls, ArrayView1D<int, Stride1D.Dense> gridData, SGrid sGrid)
        {
            var pixelInformation = new PixelInformaton(); 
            for (int sampleId = 0; sampleId < samples; sampleId++)
            {
                lightRay.Destination = PTGI_Random.GetPointInRadius(index, seedArray, 10).AddNew(lightRay.Source);

                SPoint rayDirection = lightRay.GetDirection();
                rayDirection.Normalize();
                rayDirection.Multiply(10000);

                lightRay.Destination.Add(rayDirection);

                SLine wallToIgnore = new SLine();
                var rayTraceResult = CUDA_TraceRay(index, seedArray, bitmap, bounceLimit, false, walls, lightRay, wallToIgnore, gridData, sGrid);

                pixelInformation.pixelColor.Add(rayTraceResult.pixelColor);
            }
            return pixelInformation;
        }

        private static RayTraceResult CUDA_TraceRay(Index1D index, ArrayView1D<int, Stride1D.Dense> seedArray, Bitmap bitmap, int bounceLimit, bool originDensitySwap,
            ArrayView1D<SLine, Stride1D.Dense> walls, SLine lightRay, SLine wallToIgnore, ArrayView1D<int, Stride1D.Dense> gridData, SGrid sGrid)
        {
            var rayTraceResult = new RayTraceResult();
            rayTraceResult.pixelColor = new Color();
            rayTraceResult.pixelColor.SetColor(1, 1, 1);

            const float reflectionAreaSize = 10;
            for (var bounceIndex = 0; bounceIndex < bounceLimit; bounceIndex++)
            {
                var gridTraversalResult = sGrid.TraverseGrid(lightRay, bitmap.WallsCount, gridData, walls, wallToIgnore);
                if (gridTraversalResult.IntesectedWall.HasValue != 1)
                {
                    rayTraceResult.pixelColor.SetColor(0, 0, 0);
                    return rayTraceResult;
                }
                else if(gridTraversalResult.IsClosestIntersectionLight)
                {
                    rayTraceResult.pixelColor.TintWith(gridTraversalResult.IntesectedWall.Color.GetRescaled(255), gridTraversalResult.IntesectedWall.EmissionStrength);
                    return rayTraceResult;
                }
                else
                {
                    if(lightRay.Source.IsEqualTo(gridTraversalResult.IntersectionSPoint))
                    {
                        rayTraceResult.pixelColor.SetColor(0, 0, 0);
                        return rayTraceResult;
                    }
                    wallToIgnore = gridTraversalResult.IntesectedWall;

                    var nextRayDirection = TraceRayUtility.NextRayDirection(index, seedArray, gridTraversalResult.IntesectedWall, wallToIgnore, lightRay.Source, gridTraversalResult.IntersectionSPoint, reflectionAreaSize, originDensitySwap);
                    originDensitySwap = nextRayDirection.SwapDensity;

                    if(gridTraversalResult.IntesectedWall.ReflectivnessType == 4)
                    {
                        rayTraceResult.pixelColor.Multiply(gridTraversalResult.IntesectedWall.Color.GetRescaled(255));
                    }
                    else
                    {
                        rayTraceResult.pixelColor.Multiply(gridTraversalResult.IntesectedWall.Color/*.GetRescaled(XMath.PI)*/.GetRescaled(255));
                    }

                    if (rayTraceResult.pixelColor.IsDim())
                    {
                        rayTraceResult.pixelColor.SetColor(0, 0, 0);
                        return rayTraceResult;
                    }

                    lightRay.Source = gridTraversalResult.IntersectionSPoint;
                    lightRay.Destination = nextRayDirection.Destination;
                }
            }

            rayTraceResult.pixelColor.SetColor(0, 0, 0);
            return rayTraceResult;
        }
    }
}
