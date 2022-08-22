using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct SLineNormals
    {
        public SPoint NormalUp;
        public SPoint NormalDown;

        public SLineNormals Normalize()
        {
            NormalUp.Normalize();
            NormalDown.Normalize();
            return this;
        }
    }
}
