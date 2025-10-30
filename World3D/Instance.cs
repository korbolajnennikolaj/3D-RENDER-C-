using System.Globalization;
using System.Collections.Concurrent;
using System.Text;
using Geometry;
using System.Diagnostics;
using static Shading.Tools;

namespace World3D{
    public class Instance{
        public d3DMatrix dTransform;

        public Instance(){
            dTransform = new d3DMatrix();
        }
    }

    public class MeshInstance : Instance{
        public Mesh mesh;
        public dVector3 Scale;
        public dColor Color;

        public MeshInstance() : base(){
            mesh = new Mesh();
            Scale = new dVector3(1, 1, 1);
            Color = new dColor();
        }
    }

    public class DirectLight : Instance{
        public float Power;

        public DirectLight() : base(){
            Power = 0.1f;
        }

        public dColor AbjustColor(d3DTriangle triangle, d3DMatrix CameraMatrix){
            dVector3 Normal = dVector3.Normal(triangle.Point1, triangle.Point2, triangle.Point3);
            float Scalar = -MathF.Min(dVector3.DotProduct((dTransform * CameraMatrix).Z, Normal), 0.0f);

            dColor Remains = triangle.Color * Power;
            dColor Ans = new dColor();

            Ans.R = (byte)MathF.Min(triangle.Color.R * Scalar + Remains.R, 255);
            Ans.G = (byte)MathF.Min(triangle.Color.G * Scalar + Remains.G, 255);
            Ans.B = (byte)MathF.Min(triangle.Color.B * Scalar + Remains.B, 255);

            return Ans;
        }
    }

    public class Mesh{
        public List<float[]> VertexList;
        public List<int[]> TriangleList;

        public Mesh(List<float[]> vertexList, List<int[]> triangleList){
            VertexList = vertexList;
            TriangleList = triangleList;
        }

        public Mesh() : this(new List<float[]>{}, new List<int[]>{}){}

        public List<d3DTriangle> BuildMesh(d3DMatrix Matrix, dVector3 Scale, dColor Color){
            List<d3DTriangle> triangles = new List<d3DTriangle>();

            for (int i = 0; i < TriangleList.Count; i++){
                triangles.Add(new d3DTriangle(
                    (Matrix * (new dVector3(VertexList[TriangleList[i][0]][0], VertexList[TriangleList[i][0]][1], VertexList[TriangleList[i][0]][2]) * Scale)).Origin,
                    (Matrix * (new dVector3(VertexList[TriangleList[i][1]][0], VertexList[TriangleList[i][1]][1], VertexList[TriangleList[i][1]][2]) * Scale)).Origin,
                    (Matrix * (new dVector3(VertexList[TriangleList[i][2]][0], VertexList[TriangleList[i][2]][1], VertexList[TriangleList[i][2]][2]) * Scale)).Origin, 
                    Color));
            }
            
            return triangles;
        }

        public ConcurrentBag<d3DTriangle> BuildMeshParallel(d3DMatrix Matrix, dVector3 Scale, dColor Color){
            ConcurrentBag<d3DTriangle> concurrentTriangles = new ConcurrentBag<d3DTriangle>();

            int CountCpu = Environment.ProcessorCount;
            int InOneCpu = TriangleList.Count / (CountCpu-1);
            
            Parallel.For(0, CountCpu, CpuNumber => {
                int From = InOneCpu * CpuNumber;
                int To;

                if (CpuNumber < CountCpu-1) To = InOneCpu * CpuNumber + InOneCpu;
                else To = TriangleList.Count;

                for (int i = From; i < To; i++){
                    concurrentTriangles.Add(new d3DTriangle(
                        (Matrix * (new dVector3(VertexList[TriangleList[i][0]][0], VertexList[TriangleList[i][0]][1], VertexList[TriangleList[i][0]][2]) * Scale)).Origin,
                        (Matrix * (new dVector3(VertexList[TriangleList[i][1]][0], VertexList[TriangleList[i][1]][1], VertexList[TriangleList[i][1]][2]) * Scale)).Origin,
                        (Matrix * (new dVector3(VertexList[TriangleList[i][2]][0], VertexList[TriangleList[i][2]][1], VertexList[TriangleList[i][2]][2]) * Scale)).Origin,
                        Color));
                }
            });

            return concurrentTriangles;
        }

        public void ImportObj(string Path){
            VertexList.Clear();
            TriangleList.Clear();

            string FileContent = "";

            using (FileStream fileStream = File.Open(Path, FileMode.Open)){
                byte[] Buffer = new byte[fileStream.Length];

                fileStream.Read(Buffer, 0, Buffer.Length);
                FileContent = Encoding.Default.GetString(Buffer);
            };

            string[] Lines = FileContent.Split("\n");

            foreach (string Line in Lines){
                if (Line.StartsWith("v ")){
                    string[] ValuesString = Line.Replace("v ", "").Split(" ");
                    
                    VertexList.Add(new float[]{
                        float.Parse(ValuesString[0], CultureInfo.InvariantCulture.NumberFormat), 
                        float.Parse(ValuesString[1], CultureInfo.InvariantCulture.NumberFormat), 
                        float.Parse(ValuesString[2], CultureInfo.InvariantCulture.NumberFormat)});

                }
                else if (Line.StartsWith("f ")){
                    string[] ValuesString = Line.Replace("f ", "").Split(" ");
                    
                    if (ValuesString.Length == 3){
                        int[] TriangleInt = new int[3]{0, 0, 0};

                        for (int i = 0; i <= 2; i++){
                            TriangleInt[i] = int.Parse(ValuesString[i].Split("/")[0]) - 1;
                        }
                        TriangleList.Add(TriangleInt);
                    }
                    else if (ValuesString.Length == 4){
                        TriangleList.Add(new int[]{int.Parse(ValuesString[0].Split("/")[0]) - 1, 
                                                   int.Parse(ValuesString[1].Split("/")[0]) - 1, 
                                                   int.Parse(ValuesString[2].Split("/")[0]) - 1});
                        TriangleList.Add(new int[]{int.Parse(ValuesString[0].Split("/")[0]) - 1, 
                                                   int.Parse(ValuesString[2].Split("/")[0]) - 1, 
                                                   int.Parse(ValuesString[3].Split("/")[0]) - 1});
                    }
                }
            }
        }
    }

    public static class MeshFigures{
        public static class Cube{
            public static List<float[]> VertexList = new List<float[]>(){
                new float [] { -0.5f, 0.5f, -0.5f},
                new float [] { 0.5f,  0.5f, -0.5f},
                new float [] { 0.5f, -0.5f, -0.5f},
                new float [] {-0.5f, -0.5f, -0.5f},
                new float [] {-0.5f,  0.5f,  0.5f},
                new float [] { 0.5f,  0.5f,  0.5f},
                new float [] { 0.5f, -0.5f,  0.5f},
                new float [] {-0.5f, -0.5f,  0.5f}
            };

            public static List<int[]> TriangleList = new List<int[]>() {
                new int[] { 0, 1, 2 },
                new int[] { 0, 2, 3 },
                new int[] { 2, 1, 5 },
                new int[] { 2, 5, 6 },
                new int[] { 3, 2, 6 },
                new int[] { 3, 6, 7 },
                new int[] { 0, 3, 7 },
                new int[] { 0, 7, 4 },
                new int[] { 1, 0, 4 },
                new int[] { 1, 4, 5 },
                new int[] { 6, 5, 4 },
                new int[] { 6, 4, 7 }
            };
        }
    }
}
