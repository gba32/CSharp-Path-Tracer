using System;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal abstract class Construction : IObject
    {

        protected Func<Intersection, Material> ColourFunc;

        public Construction(Func<Intersection, Material> colourFunc)
        {
            ColourFunc = colourFunc;
        }

        public Material GetMaterial(Intersection intersection)
        {
            return ColourFunc(intersection);
        }
        public abstract Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection);

        public void SetColouringFunction(Func<Intersection, Material> func)
        {
            ColourFunc = func;
        }
    }
}
