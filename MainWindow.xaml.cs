using CSharp_Path_Tracer.Renderer;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Plane = CSharp_Path_Tracer.Renderer.Plane;

namespace CSharp_Path_Tracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private WriteableBitmap writeableBitmap;
        private Renderers.Renderer renderer;
        private uint frame = 0;

        public MainWindow()
        {
            InitializeComponent();
            Show();

            // Configures image output and sets the source to the bitmap that will be written to
            RenderOptions.SetBitmapScalingMode(ViewPort, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(ViewPort, EdgeMode.Aliased);

            writeableBitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Bgr32, null);

            ViewPort.Source = writeableBitmap;
            ViewPort.Stretch = Stretch.None;
            ViewPort.HorizontalAlignment = HorizontalAlignment.Left;
            ViewPort.VerticalAlignment = VerticalAlignment.Top;

            Camera camera = new Camera(new Vector3(0.0f, 0.0f, -2.0f), 1.0f);
            List<ILight> lights = new List<ILight>();
            List<IObject> objects = new List<IObject>();

            Vector3 colour = new Vector3(0.64f, 0.2f, 0.44f);
            SphereLight sphereLight = new SphereLight(Vector3.UnitY, colour, 0.2f);
            lights.Add(sphereLight);

            Sphere sphere = new Sphere(0.5f * Vector3.UnitY, Vector3.UnitX, 0.2f);
            Plane plane = new Plane(Vector3.UnitY, 0.5f, Vector3.UnitZ);
            objects.Add(sphere);
            objects.Add(plane);

            Scene scene = new Scene(camera, lights, objects);
            renderer = new Renderers.Renderer(writeableBitmap, scene, new Tuple<uint, uint>((uint)ActualWidth, (uint)ActualHeight));
            CompositionTarget.Rendering += UpdateScreen;
        }

        private void UpdateScreen(object? sender, EventArgs e)
        {
            renderer.Draw(frame++);
        }
    }
}
