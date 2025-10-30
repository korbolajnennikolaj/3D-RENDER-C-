using System.Data.SqlTypes;
using Geometry;
using SFML.Graphics;
using static Shading.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections.Concurrent;


namespace World3D{
    public struct d3DTriangle{
        public dVector3 Point1;
        public dVector3 Point2;
        public dVector3 Point3;

        public dColor Color;

        public d3DTriangle(dVector3 point1, dVector3 point2, dVector3 point3, dColor color){
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;

            Color = color;
        }

        public d3DTriangle() : this(new dVector3(), new dVector3(), new dVector3(), new dColor()){}
    }

    public struct Polygon{
        public dVector2 Point1;
        public dVector2 Point2;
        public dVector2 Point3;

        public dColor Color;

        public Polygon(dVector2 point1, dVector2 point2, dVector2 point3, dColor color){
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;

            Color = color;
        }

        public Polygon(): this(new dVector2(), new dVector2(), new dVector2(), new dColor()){}
    }

    public class Scene{
        public Dictionary<string, Instance> Instances;
        public Camera RenderCamera;
        public DirectLight Light;

        public bool UseMultiRender;

        public Scene(Camera renderCamera){
            Instances = new Dictionary<string, Instance>();
            RenderCamera = renderCamera;
            UseMultiRender = false;
            Light = new DirectLight();
        }

        public Scene() : this(new Camera()){}

        public List<Polygon> UpdateRender(){
            List<Polygon> Polygons = new List<Polygon>();
            d3DMatrix InverseCameraMatrix = RenderCamera.dTransform.Inverse();
            InverseCameraMatrix.Origin = new dVector3();
            List<d3DTriangle> triangles = new List<d3DTriangle>();

            foreach(var item in Instances){
                if (item.Value is MeshInstance meshInstance){
                    d3DMatrix ViewMatrix = InverseCameraMatrix * (meshInstance.dTransform - RenderCamera.dTransform.Origin);
                    triangles.AddRange(meshInstance.mesh.BuildMesh(ViewMatrix, meshInstance.Scale, meshInstance.Color));
                }
            }
            
            triangles.Sort((a, b) => (b.Point1.Z + b.Point2.Z + b.Point3.Z).CompareTo(a.Point1.Z + a.Point2.Z + a.Point3.Z));
            
            foreach (d3DTriangle triangle in triangles){
                if (Camera.IsPolygonVisible(triangle.Point1, triangle.Point2, triangle.Point3)){
                    List<d3DTriangle> Clipped = Camera.TriangleClipping(new dVector3(0, 0, 0.001f), new dVector3(0, 0, 1), triangle);

                    for (int n = 0; n < Clipped.Count; n++){
                        Polygons.Add(new Polygon(
                            RenderCamera.Get2DVectorFrom3D(Clipped[n].Point1),
                            RenderCamera.Get2DVectorFrom3D(Clipped[n].Point2),
                            RenderCamera.Get2DVectorFrom3D(Clipped[n].Point3),
                            Light.AbjustColor(Clipped[n], RenderCamera.dTransform)));
                    }
                }   
            }

            return Polygons;
        }

        public ConcurrentBag<Polygon> UpdateRenderParallel(){
            ConcurrentBag<Polygon> Polygons = new ConcurrentBag<Polygon>();
            d3DMatrix InverseCameraMatrix = RenderCamera.dTransform.Inverse();
            InverseCameraMatrix.Origin = new dVector3();
            ConcurrentBag<d3DTriangle> triangles = new ConcurrentBag<d3DTriangle>();

            foreach(var item in Instances){
                if (item.Value is MeshInstance meshInstance){
                    d3DMatrix ViewMatrix = InverseCameraMatrix * (meshInstance.dTransform - RenderCamera.dTransform.Origin);
                    Parallel.ForEach(meshInstance.mesh.BuildMeshParallel(ViewMatrix, meshInstance.Scale, meshInstance.Color), item => triangles.Add(item));
                }
            }

            List<d3DTriangle> trianglesList = new List<d3DTriangle>(triangles.AsParallel().OrderBy(t => t.Point1.Z + t.Point2.Z + t.Point3.Z));

            int CountCpu = Environment.ProcessorCount;
            int InOneCpu = trianglesList.Count / (CountCpu-1);
            
            ParallelLoopResult Task = Parallel.For(0, CountCpu, CpuNumber => {
                int From = InOneCpu * CpuNumber;
                int To;

                if (CpuNumber < CountCpu-1) To = InOneCpu * CpuNumber + InOneCpu;
                else To = trianglesList.Count;

                for (int i = From; i < To; i++){
                    if (Camera.IsPolygonVisible(trianglesList[i].Point1, trianglesList[i].Point2, trianglesList[i].Point3)){
                        List<d3DTriangle> Clipped = Camera.TriangleClipping(new dVector3(0, 0, 0.001f), new dVector3(0, 0, 1), trianglesList[i]);

                        for (int n = 0; n < Clipped.Count; n++){
                            Polygons.Add(new Polygon(
                                RenderCamera.Get2DVectorFrom3D(Clipped[n].Point1),
                                RenderCamera.Get2DVectorFrom3D(Clipped[n].Point2),
                                RenderCamera.Get2DVectorFrom3D(Clipped[n].Point3),
                                Light.AbjustColor(Clipped[n], RenderCamera.dTransform)));
                        }
                    }
                }
            });

            return Polygons;
        }
    }
}
