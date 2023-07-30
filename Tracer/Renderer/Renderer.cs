﻿using CSharp_Path_Tracer.Tracer.Lights;
using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSharp_Path_Tracer.Tracer
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
                    // Accounts for the top y-value being 0 in screen space, but 1 in ndc
                    uint flippedY = (uint)(Dimensions.Item2 - 1 - y);

                    // Blends frames to reduce the inherent noise and stores the blended colour
                    Pixels[flippedY, x] = Vector3.Lerp(Pixels[flippedY, x], ShadePixel(x, y), 1.0f / (frame + 1.0f));
                    Data[flippedY, x] = IObject.VectorToUInt(255.0f * Pixels[flippedY, x]);
                }
            });

            // Overwrites the pixels in the bitmap to update the screen
            Int32Rect screenRect = new Int32Rect(0, 0, (int)width, (int)height);
            int stride = ((int)width * Screen.Format.BitsPerPixel + 7) / 8;
            Screen.WritePixels(screenRect, Data, stride, 0, 0);
        }

        private Vector3 ShadePixel(float x, float y)
        {
            const int BOUNCE_COUNT = 1;
            float width = Dimensions.Item1;
            float height = Dimensions.Item2;
            float aspectRatio = width / height;

            float ndcX = 2.0f * (x / width) - 1.0f;
            float ndcY = 2.0f * (y / height) - 1.0f;
            ndcY /= aspectRatio;

            // Sends a ray through the given pixel that bounces a specific number of times
            Vector3 rayDirection = Vector3.Normalize(new Vector3(ndcX, ndcY, 1.0f));
            Vector3 colour = TraceRay(Scene.Camera.Position, rayDirection, BOUNCE_COUNT, 0.0f);

            // Tonemaps the colour using reinhard tone mapping
            float oldLuminance = Luminance(colour);
            float ratio = 1.0f / (1.0f + oldLuminance);
            if(ratio < 0.0f) Debug.WriteLine("X: {0}, Y: {1}", ndcX, ndcY);
            Vector3 toneMapped = colour * ratio;

            // Gamma correction
            return VectorUtil.Pow(toneMapped, 1.0f / 2.2f);
        }

        private Vector3 TraceRay(Vector3 rayOrigin, Vector3 rayDirection, int bounce, float distance)
        {
            var (intersection, obj) = Scene.IntersectScene(rayOrigin, rayDirection, float.PositiveInfinity);

            if (obj == null || intersection == null)
            {
                // If the ray misses the sky box is sampled for a given direction
                return Scene.SampleSky(rayOrigin, rayDirection) / (1.0f + distance * distance);
            }
            else if (Scene.IsLight(obj))
            {
                // If the ray hits a light the colour of the light is attenuated and returned
                float totalDistance = intersection.Distance + distance;
                return obj.GetMaterial(intersection).Albedo / (1.0f + totalDistance * totalDistance);
            }


            Random random = new Random();

            // Can be replaced with a physically based BRDF
            Vector3 BRDF = obj.GetMaterial(intersection).Albedo / MathF.PI;
            Vector3 directIllumination = new Vector3();

            const uint SAMPLES = 5;

            // Samples a random point on 
            for (int lightIndex = 0; lightIndex < Scene.Lights.Count; lightIndex++)
            {
                for (int i = 0; i < SAMPLES; i++)
                {
                    directIllumination += DirectIllumination(random, Scene.Lights[lightIndex], obj, intersection);
                }
                
            }
            if(Scene.Lights.Count > 0) directIllumination /= SAMPLES * Scene.Lights.Count;

            Vector3 indirectIllumination = Vector3.Zero;
            // Samples different diffuse reflections using a cosine weighted 
            if (bounce > 0)
            {
                Vector3 diffuseReflection = DiffuseReflection(random, intersection.Normal);
                float cosR = Vector3.Dot(intersection.Normal, diffuseReflection);
                indirectIllumination = TraceRay(intersection.Position + EPSILON * diffuseReflection, diffuseReflection, bounce - 1, distance + intersection.Distance) * cosR;
            }
            return directIllumination + MathF.Tau * BRDF * indirectIllumination;
        }

        private Vector3 DirectIllumination(Random random, ILight light, IObject obj, Intersection intersection)
        {
            Intersection lightIntersection = light.GenerateRandomSurfacePoint(random, intersection.Position);
            Vector3 lightDir = Vector3.Normalize(lightIntersection.Position - intersection.Position);
            float cosO = Vector3.Dot(-lightDir, lightIntersection.Normal);
            float cosI = Vector3.Dot(lightDir, intersection.Normal);

            if (cosO <= 0 || cosI <= 0) return Vector3.Zero;
            // Shadow ray   
            var (_, shadowObj) = Scene.IntersectScene(intersection.Position + EPSILON * lightDir, lightDir, lightIntersection.Distance - 2.0f * EPSILON);
            if (shadowObj != null)
            {
                return Vector3.Zero;
            }

            Vector3 BDRF = obj.GetMaterial(intersection).Albedo / MathF.PI;
            float surfaceArea = light.GetSurfaceArea();
            float solidAngle = cosO * surfaceArea / (lightIntersection.Distance * lightIntersection.Distance);
            solidAngle = solidAngle > surfaceArea ? surfaceArea : solidAngle; 
            Vector3 x = BDRF * light.GetMaterial(lightIntersection).Albedo * solidAngle * cosI;
            return x;
        }

        private static Matrix4x4 NormalAlign(Vector3 normal, Vector3 up)
        {
            Vector3 c = Vector3.Cross(up, normal);
            float sinTheta = c.Length();
            float cosTheta = Vector3.Dot(normal, up);
            Matrix4x4 productMatrix = new Matrix4x4(
                    0.0f, c.Z, -c.Y, 0.0f,
                    -c.Z, 0.0f, c.X, 0.0f,
                    c.Y, -c.X, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );
            Matrix4x4 squaredProductMatrix = productMatrix * productMatrix;
            float k = 1.0f / (1.0f + cosTheta);
            Matrix4x4 rotation = (Matrix4x4.Identity + productMatrix + squaredProductMatrix * k);
            return rotation;
        }

        private static Vector3 DiffuseReflection(Random random, Vector3 normal)
        {
            float u = random.NextSingle();
            float v = random.NextSingle();
            float r = MathF.Sqrt(u);
            float phi = 2.0f * MathF.PI * v;
            Vector3 rayDirection3 = Vector3.Normalize(new Vector3(r * MathF.Cos(phi), r * MathF.Sin(phi), MathF.Sqrt(1.0f - u)));
            float dot = Vector3.Dot(Vector3.UnitZ, normal);
            if (float.IsNaN(dot)) Debug.WriteLine(normal);
            float sign = MathF.Sign(dot);
            if (MathF.Abs(dot) == 1.0f) return dot * rayDirection3;
            Matrix4x4 normalAlignmentMatrix = NormalAlign(normal, Vector3.UnitZ);
            rayDirection3 = Vector3.Transform(rayDirection3, normalAlignmentMatrix);
            float dot2 = Vector3.Dot(rayDirection3, normal);
            if (dot2 < 0.0f)
            {
                Debug.WriteLine("Normal: {0}", normal);
                Debug.WriteLine("Dot: {0}", dot2);
            }
            return rayDirection3;
        }

        private static float Luminance(Vector3 colour)
        {
            Vector3 coefficients = new Vector3(0.2126f, 0.7152f, 0.0722f);
            return Vector3.Dot(colour, coefficients);
        }
    }
}
