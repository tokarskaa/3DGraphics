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
        private Scene scene;
        private Camera activeCamera;
        WriteableBitmap bmp;
        public MainWindow()
        {
            InitializeComponent();
            bmp = new WriteableBitmap(1600, 1000, 0, 0, PixelFormats.Bgr32, BitmapPalettes.WebPalette);
            device = new Device(bmp);
            device.ShadingAlgorithm = new GouradShading();
            MainImage.Source = bmp;
            CreateMesh();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        private async void CreateMesh()
        {
            scene = await MeshLoader.LoadJSONFileAsync(@"C:\grafika\3DGraphics\bin\sphere3.babylon");
            scene.Meshes[scene.Meshes.Length - 1].isRotating = true;
            activeCamera = scene.Cameras.First();
        }

        void CompositionTarget_Rendering(object sender, object e)
        {
            device.Clear(0, 0, 0, 255);

            foreach (var mesh in scene.Meshes)
                if (mesh.isRotating == true)
                    mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);

            device.Render(activeCamera, scene);
            device.Present();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
                device.ShadingAlgorithm = new FlatShading();
            if (e.Key == Key.G)
                device.ShadingAlgorithm = new GouradShading();
        }
    }
}
