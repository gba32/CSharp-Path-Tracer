using CSharp_Path_Tracer.Renderer;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSharp_Path_Tracer.Renderers
{
    internal class Renderer
    {
        private WriteableBitmap Screen;
        private Scene Scene;
        private uint[,] Data;
        private Vector3[,] Pixels;
        private Tuple<uint, uint> Dimensions;

        const float EPSILON = 1E-5f;
        public Renderer(WriteableBitmap bitmap, Scene scene, Tuple<uint, uint> dimensions)
        {
            Screen = bitmap;
            Scene = scene;
            Dimensions = dimensions;
            Data = new uint[dimensions.Item2, dimensions.Item1];
            Pixels = new Vector3[dimensions.Item2, dimensions.Item1];
        }

        public void Draw(uint frame)
        {
            float width = Dimensions.Item1;
            float height = Dimensions.Item2;

            Parallel.For(0, Dimensions.Item2, (y, _) =>
            {
                for (uint x = 0; x < Dimensions.Item1; x++)
                {
                    uint posY = (uint)(Dimensions.Item2 - 1 - y);
                    Pixels[posY, x] = Vector3.Lerp(Pixels[posY, x], ShadePixel(x, y), 1.0f / (frame + 1.0f));
                    Data[posY, x] = IObject.VectorToUInt(255.0f * Pixels[posY, x]);
                }
            });

            // Overwrites the pixels in the bitmap to update the screen
            Int32Rect screenRect = new Int32Rect(0, 0, (int)width, (int)height);
            int stride = ((int)width * Screen.Format.BitsPerPixel + 7) / 8;
            Screen.WritePixels(screenRect, Data, stride, 0, 0);

        }

        private Vector3 ShadePixel(float x, float y)
        {
            float width = Dimensions.Item1;
            float height = Dimensions.Item2;
            float aspectRatio = width / height;

            float ndcX = 2.0f * (x / width) - 1.0f;
            float ndcY = 2.0f * (y / height) - 1.0f;
            ndcY /= aspectRatio;

            Vector3 rayDirection = Vector3.Normalize(new Vector3(ndcX, ndcY, 1.0f));

            return VectorUtil.Pow(TraceRay(Scene.Camera.Position, rayDirection), 1.0f / 2.2f);
        }

        private Vector3 TraceRay(Vector3 rayOrigin, Vector3 rayDirection)
        {

            var (intersection, obj) = Scene.IntersectScene(Scene.Camera.Position, rayDirection, float.PositiveInfinity);

            if (obj == null || intersection == null) return Vector3.Zero;
            else
            {
                if (Scene.IsLight(obj)) return obj.GetColour(intersection);
            }

            Random random = new Random();
            Matrix4x4 normalAlignmentMatrix = VectorUtil.NormalAlign(intersection.Normal, Vector3.UnitY);
            Vector3 diffuseReflection = VectorUtil.DiffuseReflection(random, normalAlignmentMatrix);
            Vector3 BRDF = obj.GetMaterial(intersection).Albedo / MathF.PI;
            Vector3 directIllumination = new Vector3();

            const uint SAMPLES = 20;
            for(int i = 0; i < SAMPLES; i++)
            {
                directIllumination += DirectIllumination(Scene.Lights[0], obj, intersection);
            }

            directIllumination /= SAMPLES;
            Vector3 Ei = TraceRay(intersection.Position, diffuseReflection) * Vector3.Dot(intersection.Normal, diffuseReflection);
            return directIllumination + 2.0f * MathF.PI * BRDF * Ei;
        }

        private Vector3 DirectIllumination(ILight light, IObject obj, Intersection intersection)
        {
            Random random = new Random();
            Intersection lightIntersection = light.GenerateRandomSurfacePoint(random, intersection.Position);
            Vector3 lightDir = Vector3.Normalize(lightIntersection.Position - intersection.Position);
            float cosO = Vector3.Dot(lightDir, lightIntersection.Normal);
            float cosI = Vector3.Dot(lightDir, intersection.Normal);
            //Debug.WriteLine("cosO: {0}", cosO);
            //Debug.WriteLine("cosI: {0}", cosI);
            if (cosO <= 0 || cosI <= 0) return Vector3.Zero;

            // Shadow ray
            var (_, shadowObj) = Scene.IntersectScene(intersection.Position + EPSILON * lightDir, lightDir, lightIntersection.Distance - 2.0f * EPSILON);
            if (shadowObj != null) return Vector3.Zero;

            Vector3 BDRF = obj.GetMaterial(intersection).Albedo / MathF.PI;
            float solidAngle = (cosO * light.GetSurfaceArea()) / (lightIntersection.Distance * lightIntersection.Distance);
            return BDRF * light.GetColour(lightIntersection) * solidAngle * cosI;

        }
    }


}
