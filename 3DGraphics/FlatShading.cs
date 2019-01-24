using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace _3DGraphics
{
    public class FlatShading : IShadingAlgorithm
    {

        Vector3 lightPos = new Vector3(10, 40, 0);

        public double ComputeShadingCoefficient(double gradient, ScanLine scanLine)
        {
            return scanLine.FaceNormalAndLightDotProduct;
        }

        public void InterpolateValue(ScanLine scanLineData, float gradient1, float gradient2)
        {
        }
    }
}
