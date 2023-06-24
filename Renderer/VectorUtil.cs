using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Renderer
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
            return new Vector3(MathF.Pow(v.X, p), MathF.Pow(v.Y, p), MathF.Pow(v.Z, p));
        }

        public static Matrix4x4 NormalAlign(Vector3 normal, Vector3 up)
        {
            Vector3 c = Vector3.Cross(up, normal);
            float cosTheta = Vector3.Dot(normal, up);
            Matrix4x4 productMatrix = new Matrix4x4(
                    0.0f, -c.Z, c.Y, 0.0f,
                     c.Z, 0.0f, -c.X, 0.0f,
                    -c.Y, c.X, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );
            Matrix4x4 squaredProductMatrix = productMatrix * productMatrix;
            Matrix4x4 rotation = Matrix4x4.Identity + productMatrix + squaredProductMatrix * (1.0f / (1.0f + cosTheta));
            return rotation;
        }

        public static Vector3 DiffuseReflection(Random random, Matrix4x4 normalAlignmentMatrix)
        {
            float u = (float)random.NextDouble();
            float v = (float)random.NextDouble();
            float r = MathF.Sqrt(u);
            float phi = 2.0f * MathF.PI * v;

            Vector4 rayDirection4 = Vector4.Normalize(new Vector4(r * MathF.Cos(phi), r * MathF.Sin(phi), MathF.Sqrt(1.0f - u), 1.0f));
            rayDirection4 = Vector4.Transform(rayDirection4, normalAlignmentMatrix);
            Vector3 rayDirection3 = new Vector3(rayDirection4.X, rayDirection4.Y, rayDirection4.Z);

            return Vector3.Normalize(rayDirection3);
        }

    }
}
