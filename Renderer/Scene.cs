using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Path_Tracer.Renderer
{
    internal class Scene
    {
        public List<ILight> Lights;
        List<IObject> Objects;
        public Camera Camera;
        public Scene(Camera camera, List<ILight> lights, List<IObject> objects)
        {
            Lights = lights;
            Objects = objects;
            objects.AddRange(lights);
            Camera = camera;
        }

        public Tuple<Intersection?, IObject?> IntersectScene(Vector3 rayOrigin, Vector3 rayDirection, float maxDist)
        {
            Intersection? closestIntersect = null;
            IObject? closestObject = null;
            foreach(IObject obj in Objects)
            {
                Intersection hit = obj.Intersect(rayOrigin, rayDirection);
                if(hit.Distance > 0.0f && (closestIntersect == null || hit.Distance < closestIntersect?.Distance))
                {
                    closestIntersect = hit;
                    closestObject = obj;
                }
            }
            if(closestIntersect?.Distance > maxDist)
            {
                closestIntersect = null;
                closestObject = null;
            }
            return new Tuple<Intersection?, IObject?>(closestIntersect, closestObject);
        }

        public void AddLights(List<ILight> lights)
        {
            Lights.AddRange(lights);
            Objects.AddRange(lights);
        }

        public void AddObjects(List<IObject> objects)
        {
            Objects.AddRange(objects);
        }

        public static bool IsLight(IObject? obj)
        {
            return obj is ILight;
        }
    }
}
