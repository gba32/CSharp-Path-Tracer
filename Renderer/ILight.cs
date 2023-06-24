using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Renderer
{
    internal interface ILight : IObject
    {
        public Intersection GenerateRandomSurfacePoint(Random random, Vector3 rayOrigin);

        public float GetSurfaceArea();

        // PBR methods from https://learnopengl.com/PBR/Lighting
        public static Vector3 FresnelSchlick(float cosTheta, Vector3 f0)
        {
            float clamped = VectorUtil.Clamp(1.0f - cosTheta, 0.0f, 1.0f);
            return f0 + (new Vector3(1.0f) - f0) * MathF.Pow(clamped, 5.0f);
        }

        public static float DistributionGGX(Vector3 normal, Vector3 halfway, float roughness)
        {
            float a = roughness * roughness;
            float a2 = a * a;
            float nDotH = VectorUtil.DotClamped(normal, halfway, 0.0f, float.PositiveInfinity);
            float nDotH2 = nDotH * nDotH;

            float num = a2;
            float denom = (nDotH2 * (a2 - 1.0f) + 1.0f);
            denom = MathF.PI * denom * denom;

            return num / denom;
        }

        public static float GeometrySchlickGGX(float nDotV, float roughness)
        {
            float r = 1.0f + roughness;
            float k = (r * r) / 8.0f;

            float num = nDotV;
            float denom = nDotV * (1.0f - k) + k;

            return num / denom;
        }

        public static float GeometrySmith(Vector3 normal, Vector3 view, Vector3 light, float roughness)
        {
            float nDotV = VectorUtil.DotClamped(normal, view, 0.0f, float.PositiveInfinity);
            float nDotL = VectorUtil.DotClamped(normal, light, 0.0f, float.PositiveInfinity);
            float ggx1 = GeometrySchlickGGX(nDotL, roughness);
            float ggx2 = GeometrySchlickGGX(nDotV, roughness);
            return ggx1 * ggx2;
        }
    }
}
