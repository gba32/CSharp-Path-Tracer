using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Lights
{
    internal class SphereLight : Sphere, ILight
    {
        public SphereLight(Vector3 position, float radius, Func<Intersection, Material> func) : base(position, radius, func) { }

        public Intersection GenerateRandomSurfacePoint(Random random, Vector3 rayOrigin)
        {
            // Cosine weighting distribution on thew surface of the sphere
            float u = random.NextSingle();
            float v = random.NextSingle();

            float lambda = MathF.Acos(2.0f * u - 1.0f) - MathF.PI / 2.0f;
            float phi = 2.0f * MathF.PI * v;

            float cosLambda = MathF.Cos(lambda);
            float sinLambda = MathF.Sin(lambda);
            float cosPhi = MathF.Cos(phi);
            float sinPhi = MathF.Sin(phi);
            Vector3 initialPoint = new Vector3(cosLambda * cosPhi, cosLambda * sinPhi, sinLambda) * Radius + Centre;
            Vector3 displacement = initialPoint - rayOrigin;
            Intersection intersection = Intersect(rayOrigin, Vector3.Normalize(displacement));
            return intersection;
        }

        public float GetSurfaceArea()
        {
            return 4.0f * MathF.PI * Radius * Radius;
        }

    }
}
