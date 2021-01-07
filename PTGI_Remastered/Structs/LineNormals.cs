using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGI_Remastered.Structs
{
    public struct LineNormals
    {
        public Point NormalUp;
        public Point NormalDown;

        public LineNormals Normalize()
        {
            NormalUp.Normalize();
            NormalDown.Normalize();
            return this;
        }
    }
}
