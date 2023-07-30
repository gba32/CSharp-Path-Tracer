using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Lights
{
    internal interface ILight : IObject
    {
        public Intersection GenerateRandomSurfacePoint(Random random, Vector3 rayOrigin);

        // Used to calculate the solid angle between the point and light
        public float GetSurfaceArea();
    }
}
