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

        public MemoryBuffer<Color> PixelBuffer { get; set; }
        private int _pixelBufferLength;

        public void WithContext()
        {
            if (Context == null)
                Context = new Context(ContextFlags.Force32BitFloats, ILGPU.IR.Transformations.OptimizationLevel.O2);
        }

        public void WithAccelerator(AcceleratorId GpuId, bool UseCudaRenderer)
        {
            if (Accelerator == null)
            {
                if (GpuId != null)
                    Accelerator = Accelerator.Create(Context, GpuId);
                else if (UseCudaRenderer)
                    Accelerator = new CudaAccelerator(Context);
                else
                    Accelerator = new CPUAccelerator(Context);
            }
        }

        public void SetPixelBuffer(Color[] cachePixels)
        {
            if (_pixelBufferLength != cachePixels.Length || PixelBuffer == null)
                AllocatePixelBuffer(cachePixels.Length);
            PixelBuffer.CopyFrom(cachePixels, 0, Index1.Zero, cachePixels.Length);
        }

        private void AllocatePixelBuffer(int size)
        {
            _pixelBufferLength = size;
            if (PixelBuffer != null)
                PixelBuffer.Dispose();
            PixelBuffer = Accelerator.Allocate<Color>(size);
        }
    }
}
