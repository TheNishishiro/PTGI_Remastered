using ILGPU.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public class Gpu
    {
        public AcceleratorId Id { get; set; }
        public string Name { get; set; }
    }
}
