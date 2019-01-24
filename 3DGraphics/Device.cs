using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using SharpDX;
using System.Threading.Tasks;

namespace _3DGraphics
{
    public class Device
    {
        public IShadingAlgorithm ShadingAlgorithm { get; set; }

        private PhongReflection phongReflection = new PhongReflection();

        private byte[] backBuffer;
        private double[] depthBuffer;
        private WriteableBitmap bmp;
        private readonly int renderWidth;
        private readonly int renderHeight;

        public Device(WriteableBitmap bmp)
        {
            this.bmp = bmp;
            renderWidth = bmp.PixelWidth;
            renderHeight = bmp.PixelHeight;
            backBuffer = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
            depthBuffer = new double[bmp.PixelWidth * bmp.PixelHeight];
        }

        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (var index = 0; index < backBuffer.Length; index += 4)
            {
                backBuffer[index] = b;
                backBuffer[index + 1] = g;
                backBuffer[index + 2] = r;
                backBuffer[index + 3] = a;
            }

            for (var index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = double.MaxValue;
            }
        }

        public void Present()
        {
            bmp.WritePixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight), backBuffer, bmp.BackBufferStride, 0);
        }

        public void Render(Camera activeCamera, Scene scene)
        {
            var viewMatrix = Matrix.LookAtLH(activeCamera.Position, activeCamera.Target, Vector3.UnitY);
            var projectionMatrix = Matrix.PerspectiveFovRH(0.78f, (float)bmp.PixelWidth / bmp.PixelHeight, 0.01f, 1.0f);
            phongReflection.Lights = scene.Lights;

            foreach (Mesh mesh in scene.Meshes)
            {
                var worldMatrix = Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * Matrix.Translation(mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var face in mesh.Faces)
                {
                    var vertexA = mesh.Vertices[face.A];
                    var vertexB = mesh.Vertices[face.B];
                    var vertexC = mesh.Vertices[face.C];

                    var pixelA = Project(vertexA, transformMatrix, worldMatrix);
                    var pixelB = Project(vertexB, transformMatrix, worldMatrix);
                    var pixelC = Project(vertexC, transformMatrix, worldMatrix);

                    var color = 0.8f;
                    DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1), activeCamera, scene.Lights.First().Position);
                }
            }
        }

        private Vertex Project(Vertex vertex, Matrix transMat, Matrix world)
        {
            var point2d = Vector3.TransformCoordinate(vertex.Coordinates, transMat);
            var point3dWorld = Vector3.TransformCoordinate(vertex.Coordinates, world);
            var normal3dWorld = Vector3.TransformCoordinate(vertex.Normal, world);

            var x = point2d.X * renderWidth + renderWidth / 2.0f;
            var y = -point2d.Y * renderHeight + renderHeight / 2.0f;

            return new Vertex
            {
                Coordinates = new Vector3(x, y, point2d.Z),
                Normal = normal3dWorld,
                WorldCoordinates = point3dWorld
            };
        }

        private void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Color4 color, Camera camera, Vector3 lightPos)
        {

            var data = new ScanLine { };
            data.FaceNormal = (v1.Normal + v2.Normal + v3.Normal) / 3;
            var facePoint = (v1.WorldCoordinates + v2.WorldCoordinates + v3.WorldCoordinates) / 3;
            data.FaceNormalAndLightDotProduct =
                ComputeNormalAndLightDotProduct(facePoint, data.FaceNormal,
                    lightPos);

            PhongReflection.ComputePhongReflection(data.FaceNormal, lightPos, facePoint, camera.Position);

            Vertex[] sortedVertices = SortVertices(v1, v2, v3);
            v1 = sortedVertices[0];
            v2 = sortedVertices[1];
            v3 = sortedVertices[2];

            Vector3 p1 = v1.Coordinates;
            Vector3 p2 = v2.Coordinates;
            Vector3 p3 = v3.Coordinates;

            double nl1 = ComputeNormalAndLightDotProduct(v1.WorldCoordinates, v1.Normal, lightPos);
            double nl2 = ComputeNormalAndLightDotProduct(v2.WorldCoordinates, v2.Normal, lightPos);
            double nl3 = ComputeNormalAndLightDotProduct(v3.WorldCoordinates, v3.Normal, lightPos);


            var slopeP1P2 = ComputeInverseSlope(p1, p2);

            var slopeP1P3 = ComputeInverseSlope(p1, p3);

            if (slopeP1P2 > slopeP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.CurrentY = y;

                    if (y < p2.Y)
                    {
                        data.NormalAndLightDotProductA = nl1;
                        data.NormalAndLightDotProductB = nl3;
                        data.NormalAndLightDotProductC = nl1;
                        data.NormalAndLightDotProductD = nl2;
                        ProcessScanLine(data, v1, v3, v1, v2, color);
                    }
                    else
                    {
                        data.NormalAndLightDotProductA = nl1;
                        data.NormalAndLightDotProductB = nl3;
                        data.NormalAndLightDotProductC = nl2;
                        data.NormalAndLightDotProductD = nl3;
                        ProcessScanLine(data, v1, v3, v2, v3, color);
                    }
                }
            }
            else
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.CurrentY = y;

                    if (y < p2.Y)
                    {
                        data.NormalAndLightDotProductA = nl1;
                        data.NormalAndLightDotProductB = nl2;
                        data.NormalAndLightDotProductC = nl1;
                        data.NormalAndLightDotProductD = nl3;
                        ProcessScanLine(data, v1, v2, v1, v3, color);
                    }
                    else
                    {
                        data.NormalAndLightDotProductA = nl2;
                        data.NormalAndLightDotProductB = nl3;
                        data.NormalAndLightDotProductC = nl1;
                        data.NormalAndLightDotProductD = nl3;
                        ProcessScanLine(data, v2, v3, v1, v3, color);
                    }
                }
            }
        }

        private void ProcessScanLine(ScanLine data, Vertex va, Vertex vb, Vertex vc, Vertex vd, Color4 color)
        {
            Vector3 pointA = va.Coordinates;
            Vector3 pointB = vb.Coordinates;
            Vector3 pointC = vc.Coordinates;
            Vector3 pointD = vd.Coordinates;

            var gradient1 = pointA.Y != pointB.Y ? (data.CurrentY - pointA.Y) / (pointB.Y - pointA.Y) : 1;
            var gradient2 = pointC.Y != pointD.Y ? (data.CurrentY - pointC.Y) / (pointD.Y - pointC.Y) : 1;

            int sx = (int)Interpolate(pointA.X, pointB.X, gradient1);
            int ex = (int)Interpolate(pointC.X, pointD.X, gradient2);

            double z1 = Interpolate(pointA.Z, pointB.Z, gradient1);
            double z2 = Interpolate(pointC.Z, pointD.Z, gradient2);

            ShadingAlgorithm.InterpolateValue(data, gradient1, gradient2);

           // var snl = Interpolate(data.NormalAndLightDotProductA, data.NormalAndLightDotProductB, gradient1);
           // var enl = Interpolate(data.NormalAndLightDotProductC, data.NormalAndLightDotProductD, gradient2);

            for (var x = sx; x < ex; x++)
            {
                double gradient = (x - sx) / (double)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var ndotl = ShadingAlgorithm.ComputeShadingCoefficient(gradient, data);
                var shaded = color * (float) ndotl;
                var I = (float) PhongReflection.PhongReflectionValue;
                var illuminated = new Color4(Check(shaded.Red + I), Check(shaded.Green + I), Check(shaded.Blue + I), shaded.Alpha);
                DrawPoint(new Vector3(x, data.CurrentY, (float)z), illuminated);
            }
        }

        private static float Check(float i)
        {
            if (i < 0)
            {
                return 0;
            }
            else if (i > 1)
            {
                return 1;
            }
            else
            {
                return i;
            }
        }

        private double Interpolate(double min, double max, double gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        private void DrawPoint(Vector3 point, Color4 color)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.PixelWidth && point.Y < bmp.PixelHeight)
            {
                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }

        private void PutPixel(int x, int y, double z, Color4 color)
        {
            var index = (x + y * renderWidth);
            var index4 = index * 4;

            if (depthBuffer[index] < z)
            {
                return; 
            }

            depthBuffer[index] = z;

            backBuffer[index4] = (byte)(color.Blue * 255);
            backBuffer[index4 + 1] = (byte)(color.Green * 255);
            backBuffer[index4 + 2] = (byte)(color.Red * 255);
            backBuffer[index4 + 3] = (byte)(color.Alpha * 255);
        }

        double Clamp(double value, double min = 0, double max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private double ComputeNormalAndLightDotProduct(Vector3 vertex, Vector3 normal, Vector3 lightPosition)
        {
            var lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }

        private static Vertex[] SortVertices(Vertex v1, Vertex v2, Vertex v3)
        {
            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Coordinates.Y > v3.Coordinates.Y)
            {
                var temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            Vertex[] sorted = new Vertex[3];
            sorted[0] = v1;
            sorted[1] = v2;
            sorted[2] = v3;
            return sorted;
        }

        private static double ComputeInverseSlope(Vector3 p1, Vector3 p2)
        {
            if (p2.Y - p1.Y > 0)
                return (p2.X - p1.X) / (p2.Y - p1.Y);
            return 0;
        }
    }
}