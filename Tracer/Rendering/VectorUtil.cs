using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Rendering
{
    internal static class VectorUtil
    {
        // Helper methods
        public static float Clamp(float t, float min, float max)
        {
            return MathF.Max(MathF.Min(t, max), min);
        }

        public static float DotClamped(Vector3 v1, Vector3 v2, float min, float max)
        {
            float dot = Vector3.Dot(v1, v2);

            return Clamp(dot, min, max);
        }

        public static Vector3 Pow(Vector3 v, float p)
        {
            return Pow(v, p * Vector3.One);
        }
        public static Vector3 Pow(Vector3 v, Vector3 p)
        {
            return new Vector3(MathF.Pow(v.X, p.X), MathF.Pow(v.Y, p.Y), MathF.Pow(v.Z, p.Z));
        }

        public static Vector3 Floor(Vector3 v)
        {
            return new Vector3(MathF.Floor(v.X), MathF.Floor(v.Y), MathF.Floor(v.Z));
        }

        public static Vector3 Step(Vector3 v1, Vector3 v2)
        {
            return new Vector3
                (
                    v2.X < v1.X ? 0.0f : 1.0f,
                    v2.Y < v1.Y ? 0.0f : 1.0f,
                    v2.Z < v1.Z ? 0.0f : 1.0f
                );
        }

        public static Vector3 Sign(Vector3 v)
        {
            return new Vector3(MathF.Sign(v.X), MathF.Sign(v.Y), MathF.Sign(v.Z));
        }

    }
}
