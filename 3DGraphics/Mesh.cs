using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _3DGraphics
{
    public class Mesh
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool isRotating { get; set; }
        public Mesh(string name, int verticesCount, int facesCount)
        {
            Name = name;
            Vertices = new Vertex[verticesCount];
            Faces = new Face[facesCount];
        }
    }
}