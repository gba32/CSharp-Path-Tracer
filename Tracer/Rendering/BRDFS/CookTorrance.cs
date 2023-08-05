using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Rendering.BRDFS
{
    internal class CookTorrance
    {

        public static float DistributionGGX(Vector3 normal, Vector3 halfway, float roughness)
        {
            float a = roughness * roughness;
            float a2 = a * a;
            float nDotH = Vector3.Dot(normal, halfway);
            float nDotH2 = nDotH * nDotH;

            float denom = nDotH2 * (a2 - 1.0f) + 1.0f;
            denom = MathF.PI * denom * denom;

            return a2 / denom;
        }

        public static float GeometrySchlickGGX(float nDotV, float roughness)
        {
            float r = roughness + 1.0f;
            float k = (r * r) / 8.0f;

            float denom = nDotV * (1.0f - k) + k;
            return nDotV / denom;
        }

        public static float GeometrySmith(Vector3 normal, Vector3 view, Vector3 light, float roughness)
        {
            float nDotV = VectorUtil.DotClamped(normal, view, 0.0f, 1.0f);
            float nDotL = VectorUtil.DotClamped(normal, light, 0.0f, 1.0f);
            float ggx1 = GeometrySchlickGGX(nDotL, roughness);
            float ggx2 = GeometrySchlickGGX(nDotV, roughness);


            return ggx1 * ggx2;
        }

        public static Vector3 FresnelSchlick(Vector3 f0, float cosTheta)
        {
            return f0 + (Vector3.One - f0) * MathF.Pow(VectorUtil.Clamp(cosTheta, 0.0f, 1.0f), 5.0f);
        }

    }
}
