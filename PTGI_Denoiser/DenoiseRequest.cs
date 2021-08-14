using ILGPU.Runtime;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTGI_Denoiser
{
    public class DenoiseRequest
    {
        public Bitmap bitmap { get; set; }
        public Color[] Pixels { get; set; }
        public AcceleratorId GpuId { get; set; }
        public int KernelSize { get; set; }
        public int IterationCount { get; set; }
    }
}
