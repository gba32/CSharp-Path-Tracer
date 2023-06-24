using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Renderer
{
    internal record Material(Vector3 Albedo, float Metallic, float Roughness, float AO);
}
