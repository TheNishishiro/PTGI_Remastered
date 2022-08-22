using System;
using System.Linq;
using System.Threading;
using ILGPU;
using ILGPU.Backends.OpenCL;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Algorithms;
using PTGI_Remastered.Inputs;
using PTGI_Remastered.Structs;
using PTGI_Remastered.Utilities;
using System.Threading.Tasks;
using ILGPU.Runtime.OpenCL;

namespace PTGI_Remastered.Cache
{
    internal class TempCache
    {
        public Context Context { get; private set; }
        public Accelerator Accelerator { get; private set; }
        
        public MemoryBuffer1D<Color, Stride1D.Dense> PixelBuffer { get; set; }
        public MemoryBuffer1D<int, Stride1D.Dense> SeedBuffer { get; set; }
        public MemoryBuffer1D<SLine, Stride1D.Dense> WallBuffer { get; set; }
        public MemoryBuffer1D<int, Stride1D.Dense> GridDataBuffer { get; set; }
        public SGrid SGridCached { get; set; }
       
        private int _pixelBufferLength;
        private int _wallBufferLength;
        private int _objectBufferLength;
        private int _gridDividerBufferSize;
        private bool _updatePixelBuffer;
        private bool _previousEnclousureOptionState;
        private AcceleratorType _previouslyUsedAccelerator;
        private int _previouslyDeviceId;
        private bool _isLiveDisplay = true; // TODO: constantly update?

        private Color[] cachePixels; 
        private int[] gridLocalData { get; set; }

        public void WithContext(int deviceId, AcceleratorType acceleratorType)
        {
            if (_previouslyUsedAccelerator == acceleratorType && _previouslyDeviceId == deviceId) return;

            _previouslyUsedAccelerator = acceleratorType;
            _previouslyDeviceId = deviceId;
            ResetBuffers();
            var contextBuilder = Context.Create().EnableAlgorithms().Math(MathMode.Fast32BitOnly).Optimize(OptimizationLevel.O2);
            switch (acceleratorType)
            {
                case AcceleratorType.Cuda:
                    Context = contextBuilder.Cuda().ToContext();
                    Accelerator = Context.CreateCudaAccelerator(deviceId);
                    break;
                case AcceleratorType.CPU:
                    Context = contextBuilder.CPU().ToContext();
                    Accelerator = Context.CreateCPUAccelerator(deviceId, CPUAcceleratorMode.Auto, ThreadPriority.Highest);
                    break;
                case AcceleratorType.OpenCL:
                    Context = contextBuilder.OpenCL().ToContext();
                    Accelerator = Context.CreateCudaAccelerator(deviceId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(acceleratorType), acceleratorType, null);
            }
        }

        private void ResetBuffers()
        {
            PixelBuffer = null;
            SeedBuffer = null;
            WallBuffer = null;
            GridDataBuffer = null;
        }
        
        public void WithEnclosureDetection(Bitmap bitmap, RenderSpecification renderSpecification)
        {
            var hasUpdatedPolygon = renderSpecification.Objects.Any(x => x.IsUpdated);
            if (bitmap.Size == _pixelBufferLength && 
                renderSpecification.Objects.Length == _objectBufferLength && 
                _previousEnclousureOptionState == renderSpecification.IgnoreEnclosedPixels &&
                !hasUpdatedPolygon)
                return;

            cachePixels = new Color[bitmap.Size];
            _pixelBufferLength = bitmap.Size;
            _objectBufferLength = renderSpecification.Objects.Length;
            _previousEnclousureOptionState = renderSpecification.IgnoreEnclosedPixels;
            _updatePixelBuffer = true;

            if (hasUpdatedPolygon)
            {
                foreach (var obj in renderSpecification.Objects.Where(x => x.IsUpdated))
                {
                    obj.IsUpdated = false;
                }
            }

            if (!renderSpecification.IgnoreEnclosedPixels)
                return;

            Parallel.For(0, bitmap.Size, (i) =>
            {
                var point = PTGI_Math.Convert1dIndexTo2dPointClass(bitmap, i);
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
        
        public void SetWallBuffer(SLine[] walls)
        {
            if (_wallBufferLength != walls.Length || WallBuffer == null)
                AllocateWallBuffer(walls.Length);
            WallBuffer.CopyFromCPU(walls);
        }

        public void SetGridDataBuffer(SLine[] walls, Bitmap bitmap, int gridSize)
        {
            if (_isLiveDisplay || _pixelBufferLength != bitmap.Size || GridDataBuffer == null || _updatePixelBuffer || gridLocalData == null || _gridDividerBufferSize != gridSize)
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
            WallBuffer = Accelerator.Allocate1D<SLine>(size);
        }

        private void AllocateGridDataBuffer(SLine[] walls, Bitmap bitmap, int gridSize)
        {
            _gridDividerBufferSize = gridSize;
            GridDataBuffer?.Dispose();

            SGridCached = SGridCached.Create(bitmap, gridSize);
            gridLocalData = SGridCached.CPU_FillGrid(walls);
            GridDataBuffer = Accelerator.Allocate1D<int>(gridLocalData);
        }
    }
}