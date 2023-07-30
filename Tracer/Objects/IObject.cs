using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal interface IObject
    {
        public Intersection Intersect(Vector3 rayOrigin, Vector3 rayDirection);

        public Material GetMaterial(Intersection intersection);

        public static uint VectorToUInt(Vector3 colour)
        {
            int colourData = (byte)colour.X << 16;
            colourData |= (byte)colour.Y << 8;
            colourData |= (byte)colour.Z << 0;
            return (uint)colourData;
        }
    }
}
