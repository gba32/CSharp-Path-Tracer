using CSharp_Path_Tracer.Tracer.Rendering;
using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal class SkyBox
    {
        float Size;
        Func<Vector3, float, Vector3> ColourFunc;
        public SkyBox(float size, Func<Vector3, float, Vector3> func)
        {
            Size = size;
            ColourFunc = func;
        }

        // modification of iquilez box intersection function
        public Vector3 SampleDirection(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Vector3 m = Vector3.Divide(Vector3.One, rayDirection + new Vector3(1E-15f));
            Vector3 n = m * rayOrigin;
            Vector3 k = Vector3.Abs(m) * Size;
            Vector3 t1 = -n - k;
            Vector3 t2 = -n + k;
            float tN = MathF.Max(MathF.Max(t1.X, t1.Y), t1.Z);
            float tF = MathF.Min(MathF.Min(t2.X, t2.Y), t2.Z);

            float t = tN < 0.0f ? tN : tF;

            // Do not need to consider non intersecting case as the ray originates from within the box
            Vector3 intersection = rayOrigin + t * rayDirection;

            return ColourFunc(intersection, Size);
        }

        public static float FBM(Vector3 position)
        {
            // Rotates the position to avoid direction artifacts
            Matrix4x4 rot1 = new Matrix4x4(
                -0.37f, 0.36f, 0.85f, 0.0f,
                -0.14f, -0.93f, 0.34f, 0.0f,
                0.92f, 0.01f, 0.4f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            Matrix4x4 rot2 = new Matrix4x4(
                -0.55f, -0.39f, 0.74f, 0.0f,
                0.33f, -0.91f, -0.24f, 0.0f,
                0.77f, 0.12f, 0.63f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            Matrix4x4 rot3 = new Matrix4x4(
                -0.71f, 0.52f, -0.47f, 0.0f,
                -0.08f, -0.72f, -0.68f, 0.0f,
                -0.7f, -0.45f, 0.56f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);

            return 0.5333333f * SimplexNoise(Vector3.Transform(position, rot1))
                 + 0.2666667f * SimplexNoise(Vector3.Transform(2.0f * position, rot2))
                 + 0.1333333f * SimplexNoise(Vector3.Transform(4.0f * position, rot3))
                 + 0.0666667f * SimplexNoise(8.0f * position);

        }

        //https://www.shadertoy.com/view/XsX3zB
        private static float SimplexNoise(Vector3 position)
        {
            Vector3 F3 = new Vector3(0.3333333f);
            Vector3 G3 = new Vector3(0.1666667f);

            Vector3 s = VectorUtil.Floor(position + new Vector3(Vector3.Dot(position, F3)));
            Vector3 x = position - s + new Vector3(Vector3.Dot(s, G3));
            Vector3 yzx = new Vector3(x.Y, x.Z, x.X);

            Vector3 e = VectorUtil.Step(Vector3.Zero, x - yzx);
            Vector3 zxy = new Vector3(e.Z, e.X, e.Y);

            Vector3 i1 = e * (Vector3.One - zxy);
            Vector3 i2 = Vector3.One - zxy * (Vector3.One - e);

            Vector3 x1 = x - i1 + G3;
            Vector3 x2 = x - i2 + 2.0f * G3;
            Vector3 x3 = x - Vector3.One + 3.0f * G3;

            Vector4 w = new Vector4();
            Vector4 d = new Vector4();

            w.X = x.LengthSquared();
            w.Y = x1.LengthSquared();
            w.Z = x2.LengthSquared();
            w.W = x3.LengthSquared();

            w = Vector4.Max(new Vector4(0.6f) - w, Vector4.Zero);

            d.X = Vector3.Dot(Random3(s), x);
            d.Y = Vector3.Dot(Random3(s + i1), x1);
            d.Z = Vector3.Dot(Random3(s + i2), x2);
            d.W = Vector3.Dot(Random3(s + Vector3.One), x3);

            w *= w;
            w *= w;

            d *= w;

            return Vector4.Dot(d, new Vector4(52.0f));

        }

        private static Vector3 Random3(Vector3 position)
        {
            Vector3 dot = new Vector3(17.0f, 59.4f, 15.0f);
            float j = 4096.0f * MathF.Sin(Vector3.Dot(position, dot));
            Vector3 r = new Vector3();
            r.Z = 512.0f * j - MathF.Floor(512.0f * j);
            j *= 0.125f;
            r.X = 512.0f * j - MathF.Floor(512.0f * j);
            j *= 0.125f;
            r.Y = 512.0f * j - MathF.Floor(512.0f * j);
            return r - new Vector3(0.5f);
        }
    }
}
