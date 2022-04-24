using ILGPU.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;

namespace PTGI_Remastered.Structs
{
    public class Gpu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AcceleratorType AcceleratorType { get; set; }

        public Gpu(Accelerator accelerator)
        {
            Name = accelerator.Name;
            AcceleratorType = accelerator.AcceleratorType;
            Id = 0;
        }
        
        public Gpu(CLAccelerator accelerator)
        {
            Name = accelerator.Name;
            AcceleratorType = accelerator.AcceleratorType;
            Id = accelerator.DeviceId.ToInt32();
        }
        
        public Gpu(CudaAccelerator accelerator)
        {
            Name = accelerator.Name;
            AcceleratorType = accelerator.AcceleratorType;
            Id = accelerator.DeviceId;
        }
    }
}
