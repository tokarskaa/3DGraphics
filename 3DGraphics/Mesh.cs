using SharpDX.Mathematics.Interop;
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
        public RawVector3[] Vertices { get; set; }
        public Face[] Faces { get; set; }
        public RawVector3 Position { get; set; }
        public RawVector3 Rotation { get; set; }
        public Mesh(string name, int verticesCount, int facesCount)
        {
            Name = name;
            Vertices = new RawVector3[verticesCount];
            Faces = new Face[facesCount];
        }
    }
}