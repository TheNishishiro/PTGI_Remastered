using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using PTGI_Remastered.Extensions;
using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Inputs
{
    public class RenderSpecification
    {
        public Polygon[] Objects { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int SampleCount { get; set; }
        public int BounceLimit { get; set; } 
        public int GridSize { get; set; }
        public bool IgnoreEnclosedPixels { get; set; }
        public int DeviceId { get; set; }
        public AcceleratorType AcceleratorType { get; set; }

        public Line[] GetWalls()
        {
            return Objects.ExtractWalls();
        }
    }
}
