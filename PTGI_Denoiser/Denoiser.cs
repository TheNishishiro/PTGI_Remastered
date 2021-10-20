using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using PTGI_Denoise.Denoisers;
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
            var denoiseKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1,
                Bitmap,
                ArrayView<Color>,
                int, int>(MeanDenoiser.MeanDenoiseKernel);

            denoiseKernel(pixels.Length, bitmap, pixels, kernelSize, iterationCount);
            accelerator.Synchronize();
            
            return pixels.GetAsArray();
        }

        
    }
}
