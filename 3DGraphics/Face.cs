using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;

namespace _3DGraphics
{
    public struct Face
    {
        public int A;
        public int B;
        public int C;
        public Vector3 FaceNormal;
    }
}