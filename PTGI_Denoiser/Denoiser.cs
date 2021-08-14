using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using PTGI_Denoiser.Cache;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PTGI_Denoiser
{
    public class Denoiser
    {
        private DenoiserCache _denoiserCache;

        public Denoiser()
        {
            _denoiserCache = new DenoiserCache();
        }

        public DenoiseResult Denoise(DenoiseRequest denoiseRequest)
        {
            Stopwatch denoiserTime = new Stopwatch();
            denoiserTime.Start();

            var denoiseResult = new DenoiseResult();

            _denoiserCache.WithContext();
            _denoiserCache.WithAccelerator(denoiseRequest.GpuId, true);
            _denoiserCache.SetPixelBuffer(denoiseRequest.Pixels);

            denoiseResult.Pixels = RunDenoiser(_denoiserCache.Accelerator, denoiseRequest.bitmap, _denoiserCache.PixelBuffer, denoiseRequest.KernelSize, denoiseRequest.IterationCount);
            denoiserTime.Stop();
            denoiseResult.DenoiseTime = denoiserTime.ElapsedMilliseconds;

            return denoiseResult;
        }

        private Color[] RunDenoiser(Accelerator accelerator, Bitmap bitmap, MemoryBuffer<Color> pixels, int kernelSize, int iterationCount)
        {
            var meanDenoiseKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1,
                Bitmap,
                ArrayView<Color>,
                int, int>(MeanDenoiseKernel);


            meanDenoiseKernel(pixels.Length, bitmap, pixels, kernelSize, iterationCount);
            accelerator.Synchronize();
            
            return pixels.GetAsArray();
        }

        private static void MeanDenoiseKernel(Index1 index, Bitmap bitmap, ArrayView<Color> pixels, int kernelSize, int iterationCount)
        {
            for (int i = 0; i < iterationCount; i++)
            {
                if (pixels[index].Skip == 1)
                    return;

                var array = new List<int>();

                var pixel2dCoords = PTGI_Math.Convert1dIndexTo2d(bitmap, index);
                var color = new Color();
                color.SetColor(pixels[index], 1.0f);

                float pixelsGathered = 1;
                for (int x = -kernelSize; x <= kernelSize; x++)
                {
                    for (int y = -kernelSize; y <= kernelSize; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        if (pixel2dCoords.X + x < 0 || pixel2dCoords.Y + y < 0 || pixel2dCoords.X + x >= bitmap.Width || pixel2dCoords.Y + y >= bitmap.Height)
                            continue;
                        var pixel = pixels[PTGI_Math.Convert2dIndexTo1d(pixel2dCoords.X + x, pixel2dCoords.Y + y, bitmap)];
                        if (pixel.Skip == 1)
                            continue;
                        color.Add(pixel);
                        pixelsGathered++;
                    }
                }
                color.Rescale(pixelsGathered);
                pixels[index].SetColor(color, 1.0f);
            }
        }
    }
}
