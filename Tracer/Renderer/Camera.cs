using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Tracer
{
    internal record Camera(Vector3 Position, float fov)
    {
        public Vector3 GetViewDirection(Vector3 position)
        {
            return Vector3.Normalize(position - Position);
        }
    }
}
