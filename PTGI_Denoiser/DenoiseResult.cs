using PTGI_Remastered.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTGI_Denoiser
{
    public class DenoiseResult
    {
        public Color[] Pixels { get; set; }
        public long DenoiseTime { get; set; }
    }
}
