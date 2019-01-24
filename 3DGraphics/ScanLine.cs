using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DGraphics
{
    public struct ScanLine
    {
        public int CurrentY;
        public double NormalAndLightDotProductA;
        public double NormalAndLightDotProductB;
        public double NormalAndLightDotProductC;
        public double NormalAndLightDotProductD;
        public Vector3 FaceNormal;
        public double FaceNormalAndLightDotProduct;
    }
}
