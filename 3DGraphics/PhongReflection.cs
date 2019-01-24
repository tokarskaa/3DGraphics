using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace _3DGraphics
{
    public class PhongReflection
    {
        public static Vector3 FaceNormal { get; set; }
        public List<Light> Lights { get; set; }

        private static double specularReflectionConst = 0.0;
        private static double diffuseReflectionConst = 0.2;
        private static double ambientReflectionConst = 0.2;
        private static double lightIntensity = 0.2;
        private static double shininess = 2;
        

        public static double PhongReflectionValue { get; set; }

        public static void ComputePhongReflection(Vector3 faceNormal, Vector3 lightVector, Vector3 point, Vector3 cameraSpacePosition)
        {
            var diffuse = ComputeDiffuse(faceNormal, lightVector, point);
            var specular = ComputeSpecular(lightVector, faceNormal, cameraSpacePosition, point, diffuse);
            PhongReflectionValue = ambientReflectionConst + diffuse + specular;
        }

        private static double ComputeDiffuse(Vector3 faceNormal, Vector3 lightVector, Vector3 point)
        {

            var dotProduct = ComputeNormalAndLightDotProduct(point, faceNormal, lightVector);
            return  lightIntensity * dotProduct * diffuseReflectionConst;
        }

        private static double ComputeSpecular(Vector3 lightVector, Vector3 faceNormal, Vector3 cameraSpacePosition, Vector3 point, double diffuseValue)
        {
            Vector3 viewDirection = Vector3.Normalize(-cameraSpacePosition);
            Vector3 reflectDirection = Reflection(faceNormal, -(lightVector - point));
            double specular = Vector3.Dot(viewDirection, reflectDirection);
            specular = Clamp(specular);
            specular = Math.Abs(diffuseValue) < 0.01 ? 0.0 : specular;
            return Math.Pow(specular, shininess) * lightIntensity * specularReflectionConst;
        }

        private static Vector3 Reflection(Vector3 normal, Vector3 light)
        {
            normal.Normalize();
            return light - 2 * Vector3.Dot(light, normal) * normal;
        }

        private static double ComputeNormalAndLightDotProduct(Vector3 vertex, Vector3 normal, Vector3 lightPosition)
        {
            var lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }

        private static double Clamp(double value, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
