using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace _3DGraphics
{
    public class Scene
    {
        public List<Light> Lights { get; set; }

        public List<Camera> Cameras { get; set; }

        public Mesh[] Meshes { get; set; }

        public Scene()
        {
            Lights = new List<Light>();
            Cameras = new List<Camera>();
        }
    }

    public class Light
    {
        public Vector3 Position { get; set; }
    }


}
