using System.Numerics;

namespace CSharp_Path_Tracer.Tracer.Objects
{
    internal record Material(Vector3 Albedo, float Metallic, float Roughness, float AO);
}
