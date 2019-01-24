using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace _3DGraphics
{
    public interface IShadingAlgorithm
    {
        double ComputeShadingCoefficient(double gradient, ScanLine scanLine);
        void InterpolateValue(ScanLine scanLineData, float gradient1, float gradient2);
    }
}
