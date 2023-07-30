using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal class Plane : IObject
    {
        Vector3 Normal;
        float Dot;
        Vector3 Colour;
        public Plane(Vector3 normal, float dot, Vector3 colour)
        {
            Normal = Vector3.Normalize(normal);
            Dot = dot;
            Colour = colour;
        }

        public Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float t = -(Vector3.Dot(rayOrigin, Normal) + Dot) / Vector3.Dot(rayDirection, Normal);
            if (float.IsInfinity(t) || t < 0.0f)
            {
                return new Intersection(new Vector3(), new Vector3(), -1.0f);
            }

            Vector3 position = rayOrigin + t * rayDirection;

            return new Intersection(position, -MathF.Sign(Vector3.Dot(rayDirection, Normal)) * Normal, t);
        }

        public Material GetMaterial(Intersection intersection)
        {
            // Grid colours the plane
            Vector3 colour1 = Vector3.One;
            Vector3 colour2 = colour1 / 2.0f;
            Vector3 corrected;
            float floorX = MathF.Abs(MathF.Floor(intersection.Position.X));
            float floorZ = MathF.Abs(MathF.Floor(intersection.Position.Z));
            corrected = (floorX + floorZ) % 2.0f < 1.0f ? colour1 : colour2;

            return new Material(corrected, 0.1f, 0.2f, 1.0f);
        }
    }
}
