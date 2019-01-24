using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace _3DGraphics
{
    public class GouradShading : IShadingAlgorithm
    {
        private double startNormalLightDotProduct;
        private double endNormalLightDotProduct;

        public double ComputeShadingCoefficient(double gradient, ScanLine scanLine)
        {
            return Interpolate(startNormalLightDotProduct, endNormalLightDotProduct, gradient);
        }

        public void InterpolateValue(ScanLine scanLineData, float gradient1, float gradient2)
        {
            startNormalLightDotProduct = Interpolate(scanLineData.NormalAndLightDotProductA, scanLineData.NormalAndLightDotProductB, gradient1);
            endNormalLightDotProduct = Interpolate(scanLineData.NormalAndLightDotProductC, scanLineData.NormalAndLightDotProductD, gradient2);
        }

        private double Interpolate(double min, double max, double gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        private double Clamp(double value, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
