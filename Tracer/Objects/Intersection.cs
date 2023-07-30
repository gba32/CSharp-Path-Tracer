using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal record Intersection(Vector3 Position, Vector3 Normal, float Distance);
}
