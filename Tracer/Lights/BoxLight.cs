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
    internal class BoxLight : ILight
    {
        Vector3 Position;
        Vector3 Dimensions;
        Vector3 Colour;
        public BoxLight(Vector3 position, Vector3 dimensions, Vector3 colour)
        {
            Position = position;
            Dimensions = dimensions;
            Colour = colour;
        }

        public Intersection GenerateRandomSurfacePoint(Random random, Vector3 rayOrigin)
        {
            List<Vector3> sideNormals = new List<Vector3> {
                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(0.0f, 1.0f, 0.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(-1.0f, 0.0f, 0.0f),
                    new Vector3(0.0f, -1.0f, 0.0f),
                    new Vector3(0.0f, 0.0f, -1.0f)
                };
            int side = random.Next(0, 6);
            Vector3 normal = sideNormals[side];

            Vector3 position = new Vector3(
                Dimensions.X * (random.NextSingle() - 0.5f),
                Dimensions.Y * (random.NextSingle() - 0.5f),
                Dimensions.Z * (random.NextSingle() - 0.5f));

            Vector3 projection = position * (Vector3.One - Vector3.Abs(normal)) + normal * Dimensions * 0.5f;
            Vector3 sidePosition = projection + Position;
            Intersection intersection = Intersect(rayOrigin, Vector3.Normalize(sidePosition - rayOrigin));
            return intersection;
        }

        public Material GetMaterial(Intersection intersection)
        {
            return new Material(Colour, 0.0f, 0.0f, 0.0f);
        }

        public float GetSurfaceArea()
        {
            return 2.0f * (Dimensions.X * (Dimensions.Y + Dimensions.Z) + Dimensions.Y * Dimensions.Z);
        }

        public Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection)
        {
            // translate box to the origin
            Vector3 translated = rayOrigin - Position;
            Vector3 rd = rayDirection + new Vector3(1E-15f);
            Vector3 m = Vector3.Divide(Vector3.One, rd);
            Vector3 n = m * translated;
            Vector3 k = Vector3.Abs(m) * Dimensions;
            Vector3 t1 = -n - k;
            Vector3 t2 = -n + k;

            float tN = MathF.Max(t1.X, MathF.Max(t1.Y, t1.Z));
            float tF = MathF.Min(t2.X, MathF.Min(t2.Y, t2.Z));
            if (tN > tF || tF < 0.0f) return new Intersection(new Vector3(), new Vector3(), -1.0f);

            Vector3 normal = tN > 0.0f ? VectorUtil.Step(new Vector3(tN), t1) : VectorUtil.Step(t2, new Vector3(tF));

            float t = tN > 0.0f ? tN : tF;
            normal *= -VectorUtil.Sign(rayDirection);

            Vector3 position = rayOrigin + rayDirection * t;
            return new Intersection(position, Vector3.Normalize(normal), t);
        }
    }
}
