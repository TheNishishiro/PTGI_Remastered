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
            _denoiserCache.WithAccelerator(denoiseRequest.DeviceId, true);
            _denoiserCache.SetPixelBuffer(denoiseRequest.Pixels);

            denoiseResult.Pixels = RunDenoiser(_denoiserCache.Accelerator, denoiseRequest.bitmap, _denoiserCache.PixelBuffer, denoiseRequest.KernelSize, denoiseRequest.IterationCount);
            denoiserTime.Stop();
            denoiseResult.DenoiseTime = denoiserTime.ElapsedMilliseconds;

            return denoiseResult;
        }

        private Color[] RunDenoiser(Accelerator accelerator, Bitmap bitmap, MemoryBuffer1D<Color, Stride1D.Dense> pixels, int kernelSize, int iterationCount)
        {
            var denoiseKernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                Bitmap,
                ArrayView1D<Color, Stride1D.Dense>,
                int, int>(MeanDenoiser.MeanDenoiseKernel);

            denoiseKernel((int)pixels.Length, bitmap, pixels.View, kernelSize, iterationCount);
            accelerator.Synchronize();
            
            return pixels.AsContiguous().GetAsArray();
        }

        
    }
}
