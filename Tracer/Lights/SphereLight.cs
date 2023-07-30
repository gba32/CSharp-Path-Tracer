using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer.Lights
{
    internal class SphereLight : ILight
    {
        Vector3 Centre;
        Vector3 Colour;
        float Radius;
        public SphereLight(Vector3 position, Vector3 colour, float radius)
        {
            Centre = position;
            Colour = colour;
            Radius = radius;
        }

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

        public Vector3 GetColour(Intersection intesection)
        {
            return Colour;
        }

        public Material GetMaterial(Intersection intersection)
        {
            return new Material(Colour, 0.0f, 0.0f, 1.0f);
        }

        public float GetSurfaceArea()
        {
            return 4.0f * MathF.PI * Radius * Radius;
        }

        public Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Vector3 oc = rayOrigin - Centre;
            float b = Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float h = b * b - c;
            if (h < 0.0f) return new Intersection(new Vector3(), new Vector3(), -1.0f);
            h = MathF.Sqrt(h);

            Vector3 position = rayOrigin + (-b - h) * rayDirection;
            Vector3 normal = Vector3.Normalize(position - Centre);
            return new Intersection(position, normal, -b - h);
        }
    }
}
