using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct RenderResult
    {
        public Bitmap bitmap { get; set; }
        public long RenderTime { get; set; }
        public long ProcessTime { get; set; }
        public Color[] Pixels { get; set; }
    }
}
