using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal abstract class Construction : IObject
    {
        public abstract Vector3 GetColour(Intersection intesection);

        public abstract Material GetMaterial(Intersection intersection);

        public abstract Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection);

    }
}
