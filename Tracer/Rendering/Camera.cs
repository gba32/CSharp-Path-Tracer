using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Rendering
{
    internal record Camera(Vector3 Position, float fov)
    {
        public Vector3 GetViewDirection(Vector3 position)
        {
            return Vector3.Normalize(position - Position);
        }
    }
}
