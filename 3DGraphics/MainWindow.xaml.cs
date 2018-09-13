using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DGraphics
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Device device;
        private Mesh[] meshes = new Mesh[0];
        private Camera camera = new Camera();
        WriteableBitmap bmp;
        public MainWindow()
        {
            InitializeComponent();
            bmp = new WriteableBitmap(640, 480, 0, 0, PixelFormats.Bgr32, BitmapPalettes.WebPalette);
            device = new Device(bmp);
            MainImage.Source = bmp;
            CreateMesh();
        }
        private async void CreateMesh()
        {
            //mesh.Vertices[0] = new Vector3(-1, 1, 1);
            //mesh.Vertices[1] = new Vector3(1, 1, 1);
            //mesh.Vertices[2] = new Vector3(-1, -1, 1);
            //mesh.Vertices[3] = new Vector3(-1, -1, -1);
            //mesh.Vertices[4] = new Vector3(-1, 1, -1);
            //mesh.Vertices[5] = new Vector3(1, 1, -1);
            //mesh.Vertices[6] = new Vector3(1, -1, 1);
            //mesh.Vertices[7] = new Vector3(1, -1, -1);

            //mesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            //mesh.Faces[1] = new Face { A = 1, B = 2, C = 3 };
            //mesh.Faces[2] = new Face { A = 1, B = 3, C = 6 };
            //mesh.Faces[3] = new Face { A = 1, B = 5, C = 6 };
            //mesh.Faces[4] = new Face { A = 0, B = 1, C = 4 };
            //mesh.Faces[5] = new Face { A = 1, B = 4, C = 5 };

            //mesh.Faces[6] = new Face { A = 2, B = 3, C = 7 };
            //mesh.Faces[7] = new Face { A = 3, B = 6, C = 7 };
            //mesh.Faces[8] = new Face { A = 0, B = 2, C = 7 };
            //mesh.Faces[9] = new Face { A = 0, B = 4, C = 7 };
            //mesh.Faces[10] = new Face { A = 4, B = 5, C = 6 };
            //mesh.Faces[11] = new Face { A = 4, B = 6, C = 7 };

            meshes = await device.LoadJSONFileAsync("suzanne2.babylon");

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            camera.Position = new Vector3(0, 0, 10.0f);
            camera.Target = Vector3.Zero;
        }

        // Rendering loop handler
        void CompositionTarget_Rendering(object sender, object e)
        {
            device.Clear(0, 0, 0, 255);

            // rotating slightly the cube during each frame rendered
            foreach (var mesh in meshes)
                mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);

            // Doing the various matrix operations
            device.Render(camera, meshes);
            // Flushing the back buffer into the front buffer
            device.Present();
        }
    }
}
