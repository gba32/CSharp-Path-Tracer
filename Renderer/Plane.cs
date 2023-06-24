using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Renderer
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
            float t = -(Vector3.Dot(rayOrigin, Normal) + Dot) / (Vector3.Dot(rayDirection, Normal));
            if (float.IsInfinity(t))
            {
                return new Intersection(new Vector3(), new Vector3(), -1.0f);
            }

            Vector3 position = rayOrigin + t * rayDirection;
            return new Intersection(position, Normal, t);
        }

        public Material GetMaterial(Intersection intersection)
        {
            Vector3 corrected = VectorUtil.Pow(Colour, 2.2f);
            return new Material(corrected, 0.1f, 0.2f, 1.0f);
        }
    }
}
