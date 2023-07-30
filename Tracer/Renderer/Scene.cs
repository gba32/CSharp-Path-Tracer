using CSharp_Path_Tracer.Tracer.Lights;
using CSharp_Path_Tracer.Tracer.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace CSharp_Path_Tracer.Tracer
{
    internal class Scene
    {
        public List<ILight> Lights;
        List<IObject> Objects;
        public Camera Camera;
        SkyBox Sky;
        public Scene(SkyBox sky, Camera camera, List<ILight> lights, List<IObject> objects)
        {
            Lights = lights;
            Objects = objects;
            objects.AddRange(lights);
            Camera = camera;
            Sky = sky;
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
                //Debug.WriteLine("Dist: {0}, maxDist {1}", closestIntersect?.Distance, maxDist);
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

        public Vector3 SampleSky(Vector3 rayOrigin, Vector3 rayDirection)
        {
            return Sky.SampleDirection(rayOrigin, rayDirection);
        }

        public static bool IsLight(IObject? obj)
        {
            return obj is ILight;
        }
    }
}
