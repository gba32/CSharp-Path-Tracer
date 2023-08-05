using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal class Plane : Construction
    {
        Vector3 Normal;
        float Dot;

        public Plane(Vector3 normal, float dot, Func<Intersection, Material> func) : base(func)
        {
            Normal = Vector3.Normalize(normal);
            Dot = dot;
        }


        public override Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float t = -(Vector3.Dot(rayOrigin, Normal) + Dot) / Vector3.Dot(rayDirection, Normal);
            if (float.IsInfinity(t) || t < 0.0f)
            {
                return new Intersection(new Vector3(), new Vector3(), -1.0f);
            }

            Vector3 position = rayOrigin + t * rayDirection;

            return new Intersection(position, -MathF.Sign(Vector3.Dot(rayDirection, Normal)) * Normal, t);
        }
    }
}
