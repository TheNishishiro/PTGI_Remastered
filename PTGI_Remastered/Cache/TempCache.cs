using ILGPU;
using ILGPU.Backends.OpenCL;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System.Threading.Tasks;
using Grid = PTGI_Remastered.Structs.Grid;

namespace PTGI_Remastered.Cache
{
    public class TempCache
    {
        public Context Context { get; private set; }
        public Accelerator Accelerator { get; private set; }
        
        public MemoryBuffer<Color> PixelBuffer { get; set; }
        public MemoryBuffer<int> SeedBuffer { get; set; }
        public MemoryBuffer<Line> WallBuffer { get; set; }
        public MemoryBuffer3D<int> GridDataBuffer { get; set; }
        public Grid GridCached { get; set; }
       
        private int _pixelBufferLength;
        private int _wallBufferLength;
        private int _objectBufferLength;
        private int _gridDivderBufferSize;
        private bool _updatePixelBuffer;
        private bool _previouseEnclousureOptionState;
        private bool _isLiveDisplay = true; // TODO: constantly update?

        private Color[] cachePixels; 
        private int[,,] gridLocalData { get; set; }

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

        public void WithEnclosureDetection(Bitmap bitmap, RenderSpecification renderSpecification)
        {
            // TODO: add better update logic
            if (bitmap.Size == _pixelBufferLength && renderSpecification.Objects.Length == _objectBufferLength && _previouseEnclousureOptionState == renderSpecification.IgnoreEnclosedPixels)
                return;

            cachePixels = new Color[bitmap.Size];
            _pixelBufferLength = bitmap.Size;
            _objectBufferLength = renderSpecification.Objects.Length;
            _previouseEnclousureOptionState = renderSpecification.IgnoreEnclosedPixels;
            _updatePixelBuffer = true;

            if (!renderSpecification.IgnoreEnclosedPixels)
                return;

            Parallel.For(0, bitmap.Size, (i) =>
            {
                var point = PTGI_Math.GetRaySourceFromThreadIndex(bitmap, i);
                TraceRayUtility.IsRayStartingInPolygon(point, renderSpecification.Objects, renderSpecification.SampleCount, ref cachePixels[i]);
            });
        }


        public void SetPixelBuffer()
        {
            if (_pixelBufferLength != cachePixels.Length || PixelBuffer == null || _updatePixelBuffer)
                AllocatePixelBuffer(cachePixels.Length);
            PixelBuffer.CopyFrom(cachePixels, 0, Index1.Zero, cachePixels.Length);
        }
        
        public void SetSeedBuffer(int bitmapSize)
        {
            if (_pixelBufferLength != bitmapSize || SeedBuffer == null || _updatePixelBuffer)
            {
                var seed = GenerateRandomSeed(bitmapSize);
                AllocateSeedBuffer(seed.Length);
                SeedBuffer.CopyFrom(seed, 0, Index1.Zero, seed.Length);
            }
        }

        private int[] GenerateRandomSeed(int bitmapSize)
        {
            var randomSeed = new int[bitmapSize];
            Parallel.For(0, bitmapSize, (i) =>
            {
                randomSeed[i] = PTGI_Random.Next();
            });
            return randomSeed;
        }
        
        public void SetWallBuffer(Line[] walls)
        {
            if (_wallBufferLength != walls.Length || WallBuffer == null)
                AllocateWallBuffer(walls.Length);
            WallBuffer.CopyFrom(walls, 0, Index1.Zero, walls.Length);
        }

        public void SetGridDataBuffer(Line[] walls, Bitmap bitmap, int gridSize)
        {
            if (_isLiveDisplay || _pixelBufferLength != bitmap.Size || GridDataBuffer == null || _updatePixelBuffer || gridLocalData == null || _gridDivderBufferSize != gridSize)
            {
                AllocateGridDataBuffer(walls, bitmap, gridSize);
            }
            GridDataBuffer.CopyFrom(gridLocalData, LongIndex3.Zero, Index3.Zero, GridDataBuffer.Extent);
        }

        public void Finalize()
        {
            _updatePixelBuffer = false;
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

        private void AllocateGridDataBuffer(Line[] walls, Bitmap bitmap, int gridSize)
        {
            _gridDivderBufferSize = gridSize;
            if (GridDataBuffer != null)
                GridDataBuffer.Dispose();

            GridCached = GridCached.Create(bitmap, gridSize);
            gridLocalData = GridCached.CPU_FillGrid(walls);
            GridDataBuffer = Accelerator.Allocate<int>(gridLocalData);
        }
    }
}