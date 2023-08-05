using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Lights
{
    internal class BoxLight : Box, ILight
    {
        public BoxLight(Vector3 position, Vector3 dimensions, Func<Intersection, Material> func) : base(position, dimensions, func) { }

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

        public float GetSurfaceArea()
        {
            return 2.0f * (Dimensions.X * (Dimensions.Y + Dimensions.Z) + Dimensions.Y * Dimensions.Z);
        }

    }
}
