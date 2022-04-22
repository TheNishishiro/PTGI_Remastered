using System.Linq;
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
        
        public MemoryBuffer1D<Color, Stride1D.Dense> PixelBuffer { get; set; }
        public MemoryBuffer1D<int, Stride1D.Dense> SeedBuffer { get; set; }
        public MemoryBuffer1D<Line, Stride1D.Dense> WallBuffer { get; set; }
        public MemoryBuffer3D<int, Stride3D.DenseXY> GridDataBuffer { get; set; }
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
                var point = PTGI_Math.Convert1dIndexTo2d(bitmap, i);
                TraceRayUtility.IsRayStartingInPolygon(point, renderSpecification.Objects, ref cachePixels[i]);
            });
        }


        public void SetPixelBuffer()
        {
            if (_pixelBufferLength != cachePixels.Length || PixelBuffer == null || _updatePixelBuffer)
                AllocatePixelBuffer(cachePixels.Length);
            PixelBuffer.CopyFromCPU(cachePixels);
        }
        
        public void SetSeedBuffer(int bitmapSize)
        {
            if (_pixelBufferLength == bitmapSize && SeedBuffer != null && !_updatePixelBuffer) return;
            
            var seed = GenerateRandomSeed(bitmapSize);
            AllocateSeedBuffer(seed.Length);
            SeedBuffer.CopyFromCPU(seed);
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
            WallBuffer.CopyFromCPU(walls);
        }

        public void SetGridDataBuffer(Line[] walls, Bitmap bitmap, int gridSize)
        {
            if (_isLiveDisplay || _pixelBufferLength != bitmap.Size || GridDataBuffer == null || _updatePixelBuffer || gridLocalData == null || _gridDivderBufferSize != gridSize)
            {
                AllocateGridDataBuffer(walls, bitmap, gridSize);
            }
            GridDataBuffer.CopyFromCPU(gridLocalData);
        }
        
        public void Finalize()
        {
            _updatePixelBuffer = false;
        }

        private void AllocatePixelBuffer(int size)
        {
            _pixelBufferLength = size;
            PixelBuffer?.Dispose();
            PixelBuffer = Accelerator.Allocate1D<Color>(size);
        }

        private void AllocateSeedBuffer(int size)
        {
            _pixelBufferLength = size;
            SeedBuffer?.Dispose();
            SeedBuffer = Accelerator.Allocate1D<int>(size);
        }

        private void AllocateWallBuffer(int size)
        {
            _wallBufferLength = size;
            WallBuffer?.Dispose();
            WallBuffer = Accelerator.Allocate1D<Line>(size);
        }

        private void AllocateGridDataBuffer(Line[] walls, Bitmap bitmap, int gridSize)
        {
            _gridDivderBufferSize = gridSize;
            GridDataBuffer?.Dispose();

            GridCached = GridCached.Create(bitmap, gridSize);
            gridLocalData = GridCached.CPU_FillGrid(walls);
            GridDataBuffer = Accelerator.Allocate3DDenseXY<int>(gridLocalData);
        }
    }
}