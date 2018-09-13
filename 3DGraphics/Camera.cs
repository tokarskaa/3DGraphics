using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX.Mathematics.Interop;

namespace _3DGraphics
{
    public class Camera
    {
        public RawVector3 Position { get; set; }
        public RawVector3 Target { get; set; }
    }
}