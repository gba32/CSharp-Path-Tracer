
using CSharp_Path_Tracer.Tracer;
using CSharp_Path_Tracer.Tracer.Lights;
using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Plane = CSharp_Path_Tracer.Tracer.Objects.Plane;

namespace CSharp_Path_Tracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private WriteableBitmap writeableBitmap;
        private Renderer renderer;
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

            // Scene setup
            Camera camera = new Camera(new Vector3(0.0f, 0.0f, -2.0f), 1.0f);
            List<ILight> lights = new List<ILight>();
            List<IObject> objects = new List<IObject>();

            Vector3 colour = 10.0f * new Vector3(1.0f, 1.0f, 0.1f);
            SphereLight sphereLight = new SphereLight(1.1f * Vector3.UnitY, colour, 0.3f);
            BoxLight boxLight = new BoxLight(new Vector3(-0.0f, 1.95f, 0.0f), new Vector3(0.5f, 0.05f, 0.5f), colour);
            lights.Add(boxLight);

            Sphere sphere = new Sphere(0.5f * Vector3.UnitY, Vector3.UnitX, 0.2f);
            Plane plane = new Plane(Vector3.UnitY, 0.5f, Vector3.UnitZ);
            Box box = new Box(Vector3.Zero, 2.0f * Vector3.One, Vector3.UnitX);
            objects.Add(plane);
            objects.Add(box);

            Scene scene = new Scene(new SkyBox(100.0f), camera, lights, objects);
            Tuple<uint, uint> dimensions = new Tuple<uint, uint>((uint)ActualWidth, (uint)ActualHeight);
            renderer = new Renderer(writeableBitmap, scene, dimensions);
            CompositionTarget.Rendering += UpdateScreen;
        }

        private void UpdateScreen(object? sender, EventArgs e)
        {
            renderer.Draw(frame++);
        }
    }
}
