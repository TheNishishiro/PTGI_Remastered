using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTGI_Denoiser.Cache
{
    public class DenoiserCache
    {
        public Context Context { get; private set; }
        public Accelerator Accelerator { get; private set; }

        public MemoryBuffer1D<Color, Stride1D.Dense> PixelBuffer { get; set; }
        private int _pixelBufferLength;

        public void WithContext()
        {
            Context ??= Context.Create().Optimize(OptimizationLevel.O2).Cuda().ToContext();
        }

        public void WithAccelerator(int deviceId, bool useCudaRenderer)
        {
            if (Accelerator != null) return;
            
            if (useCudaRenderer)
                Accelerator = Context.CreateCudaAccelerator(deviceId);
            else
                Accelerator = Context.CreateCPUAccelerator(deviceId);
        }


        public void SetPixelBuffer(Color[] cachePixels)
        {
            if (_pixelBufferLength != cachePixels.Length || PixelBuffer == null)
                AllocatePixelBuffer(cachePixels.Length);
            PixelBuffer.CopyFromCPU(cachePixels);
        }

        private void AllocatePixelBuffer(int size)
        {
            _pixelBufferLength = size;
            if (PixelBuffer != null)
                PixelBuffer.Dispose();
            PixelBuffer = Accelerator.Allocate1D<Color>(size);
        }
    }
}
