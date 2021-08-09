using ILGPU;
using ILGPU.Backends.OpenCL;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using PTGI_Remastered.Structs;

namespace PTGI_Remastered.Cache
{
    public class TempCache
    {
        public Context Context { get; private set; }
        public Accelerator Accelerator { get; private set; }
        
        public MemoryBuffer<Color> PixelBuffer { get; set; }
        public MemoryBuffer<int> SeedBuffer { get; set; }
        public MemoryBuffer<Line> WallBuffer { get; set; }
        
        private int _pixelBufferLength;
        private int _wallBufferLength;

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
        
        public void SetPixelBuffer(Color[] pixels)
        {
            if (_pixelBufferLength != pixels.Length || PixelBuffer == null)
                AllocatePixelBuffer(pixels.Length);
            PixelBuffer.CopyFrom(pixels, 0, Index1.Zero, pixels.Length);
        }
        
        public void SetSeedBuffer(int[] seeds)
        {
            if (_pixelBufferLength != seeds.Length || SeedBuffer == null)
                AllocateSeedBuffer(seeds.Length);
            SeedBuffer.CopyFrom(seeds, 0, Index1.Zero, seeds.Length);
        }
        
        public void SetWallBuffer(Line[] walls)
        {
            if (_wallBufferLength != walls.Length || WallBuffer == null)
                AllocateWallBuffer(walls.Length);
            WallBuffer.CopyFrom(walls, 0, Index1.Zero, walls.Length);
        }

        private void AllocatePixelBuffer(int size)
        {
            _pixelBufferLength = size;
            if (PixelBuffer != null)
                PixelBuffer.Dispose();
            PixelBuffer = Accelerator.Allocate<Color>(size);
        }

        private void AllocateSeedBuffer(int size)
        {
            _pixelBufferLength = size;
            if (SeedBuffer != null)
                SeedBuffer.Dispose();
            SeedBuffer = Accelerator.Allocate<int>(size);
        }

        private void AllocateWallBuffer(int size)
        {
            _wallBufferLength = size;
            if (WallBuffer != null)
                WallBuffer.Dispose();
            WallBuffer = Accelerator.Allocate<Line>(size);
        }
    }
}