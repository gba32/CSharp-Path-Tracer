
using CSharp_Path_Tracer.Tracer.Lights;
using CSharp_Path_Tracer.Tracer.Objects;
using CSharp_Path_Tracer.Tracer.Rendering;
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
        private Func<Intersection, Material> GridColourFunc = (intersection) =>
        {
            // Grid colours the plane
            Vector3 colour1 = Vector3.One;
            Vector3 colour2 = colour1 / 2.0f;
            Vector3 colour = Grid(colour1, colour2, intersection.Position.X, intersection.Position.Z);

            return new Material(colour, 0.1f, 0.2f, 1.0f);
        };

        private Func<Intersection, Material> TriPlanarGridColourFunc = (intersection) =>
        {
            const float k = 0.2f;
            Vector3 colour1 = Vector3.One;
            Vector3 colour2 = colour1 / 2.0f;

            Vector3 colourXY = Grid(colour1, colour2, intersection.Position.X, intersection.Position.Y);
            Vector3 colourYZ = Grid(colour1, colour2, intersection.Position.Y, intersection.Position.Z);
            Vector3 colourZX = Grid(colour1, colour2, intersection.Position.Z, intersection.Position.X);
            Vector3 w = VectorUtil.Pow(Vector3.Abs(intersection.Normal), k);
            Vector3 colour = (colourXY * w.X + colourYZ * w.Y + colourZX * w.Z) / (w.X + w.Y + w.Z);

            return new Material(colour, 0.1f, 0.2f, 1.0f);
        };

        private Func<Vector3, float, Vector3> SkyBoxColourFunc = (intersection, size) =>
        {
            Vector3 originDirection = Vector3.Normalize(intersection);
            float cosTheta = MathF.Abs(Vector3.Dot(originDirection, Vector3.UnitY));
            float ke = 4.0f;
            float kn = 3.0f;
            cosTheta = MathF.Exp(-ke * MathF.Pow(1.0f - cosTheta, kn));

            float fbm = 0.5f * SkyBox.FBM(8.0f * cosTheta * intersection / size + 8.0f * Vector3.One);
            Vector3 skyBase = new Vector3(135f, 206f, 235f) / 255.0f + new Vector3(fbm);
            Vector3 sunColour = new Vector3(252f, 229f, 112f) / 255.0f;
            Vector3 colour = Vector3.Lerp(Vector3.Zero, skyBase, cosTheta);

            return colour;
        };

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

            Vector3 colour = 2.0f * new Vector3(1.0f, 1.0f, 0.1f);
            SphereLight sphereLight = new SphereLight(1.1f * Vector3.UnitY, 0.5f, (interection) => new Material(colour, 0.0f, 0.0f, 0.0f));
            //BoxLight boxLight = new BoxLight(new Vector3(-0.0f, 1.95f, 0.0f), new Vector3(0.5f, 0.05f, 0.5f), colour);
            lights.Add(sphereLight);

            Sphere sphere = new Sphere(0.1f * Vector3.UnitY, 0.2f, TriPlanarGridColourFunc);
            Plane plane = new Plane(Vector3.UnitY, 0.5f, GridColourFunc);
            Box box = new Box(new Vector3(0.0f, -0.2f, 0.0f), new Vector3(1.5f, 0.4f, 1.5f), TriPlanarGridColourFunc);
            objects.Add(plane);
            objects.Add(sphere);

            Scene scene = new Scene(new SkyBox(100.0f, SkyBoxColourFunc), camera, lights, objects);
            Tuple<uint, uint> dimensions = new Tuple<uint, uint>((uint)ActualWidth, (uint)ActualHeight);
            RendererOptions options = new RendererOptions(10, 2, 10);
            renderer = new Renderer(writeableBitmap, scene, dimensions, options);
            CompositionTarget.Rendering += UpdateScreen;
        }

        private void UpdateScreen(object? sender, EventArgs e)
        {
            renderer.Draw();
        }

        private static Vector3 Grid(Vector3 colour1, Vector3 colour2, float x, float y)
        {
            float floorX = MathF.Abs(MathF.Floor(x));
            float floorY = MathF.Abs(MathF.Floor(y));
            Vector3 colour = (floorX + floorY) % 2.0f < 1.0f ? colour1 : colour2;

            return colour;
        }
    }
}
