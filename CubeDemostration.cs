using d2D;
using Geometry;
using static Geometry.Figures;
using World3D;

public class CubeDemonstration{
    public static void Main(){
        d2DScreen MainWindow = new d2DScreen(1024, 600);
        Scene MainScene = new Scene();

        new Thread(MainWindow.Run).Start();

        MainWindow.OnInit += ()=>{
            MeshInstance Cube = new MeshInstance();
            Cube.mesh = new Mesh(MeshFigures.Cube.VertexList, MeshFigures.Cube.TriangleList);
            Cube.dTransform.Origin = new dVector3(0, 0, 3);

            Cube.Color = new Shading.Tools.dColor(255, 0, 0);

            MainScene.RenderCamera = new Camera(MainWindow.SizeX, MainWindow.SizeY);
            MainScene.Instances.Add("Cube", Cube);
        };

        MainWindow.OnRender += (float DeltaTime)=>{
            foreach(Polygon polygon in MainScene.UpdateRender()){
                MainWindow.Draw(Triangle(polygon.Point1, polygon.Point2, polygon.Point3, polygon.Color));
            }

            MainScene.Instances["Cube"].dTransform *= d3DMatrix.AnglesYXZ(new dVector3(45, 87, 12) * DeltaTime);
        };

        MainWindow.OnClose += ()=>{
            MainWindow.Close();
        };

        MainWindow.OnResize += (float SizeX, float SizeY)=>{
            MainScene.RenderCamera.Width = SizeX;
            MainScene.RenderCamera.Height = SizeY;
        };
    }
}