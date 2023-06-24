using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Renderer
{

    internal class Sphere : IObject
    {
        private Vector3 Centre;
        private float Radius;
        private Vector3 Colour;

        public Sphere(Vector3 centre, Vector3 colour, float radius)
        {
            Centre = centre;
            Radius = radius;
            Colour = colour;
        }

        public Vector3 GetColour(Intersection intersection)
        {
            return Colour;

        }

        public void SetColour(uint red, uint green, uint blue)
        {
            Colour = new Vector3(red, green, blue);
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
            Vector3 normal = Vector3.Normalize(Centre - position);
            return new Intersection(position, normal, -b - h);

        }

        public void SetPosition(Vector3 newPosition)
        {
            Centre = newPosition;
        }

        public Material GetMaterial(Intersection intersection)
        {
            Vector3 corrected = VectorUtil.Pow(Colour, 2.2f);
            return new Material(corrected, 0.2f, 0.6f, 1.0f);
        }

    }
}