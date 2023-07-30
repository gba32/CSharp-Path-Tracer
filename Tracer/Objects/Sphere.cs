using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer.Objects
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

        public Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Vector3 oc = rayOrigin - Centre;
            float b = Vector3.Dot(oc, rayDirection);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float h = b * b - c;
            if (h < 0.0f) return new Intersection(new Vector3(), new Vector3(), -1.0f);
            h = MathF.Sqrt(h);
            float t = (-b - h) < 0.0f ? -b - h : -b + h; 
            if(t < 0.0f) return new Intersection(new Vector3(), new Vector3(), -1.0f);
            Vector3 position = rayOrigin + t * rayDirection;
            Vector3 normal = Vector3.Normalize(position - Centre);
            return new Intersection(position, normal, t);

        }

        public void SetPosition(Vector3 newPosition)
        {
            Centre = newPosition;
        }

        public Material GetMaterial(Intersection intersection)
        {
            return new Material(Colour, 0.2f, 0.6f, 1.0f);
        }

    }
}