using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{

    internal class Sphere : Construction
    {
        protected Vector3 Centre;
        protected float Radius;

        public Sphere(Vector3 centre, float radius, Func<Intersection, Material> func) : base(func)
        {
            Centre = centre;
            Radius = radius;
        }

        public override Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
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

    }
}