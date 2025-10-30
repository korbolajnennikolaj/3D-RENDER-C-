using Geometry;

namespace World3D{
    public class Camera : Instance{
        public float Near;
        public float Far;
        public float Fov;

        public float Width;
        public float Height;


        public Camera(float near, float far, float fov, float width, float height) : base(){
            Near = near;
            Far = far;
            Fov = fov;
            
            Width = width;
            Height = height;
        }

        public Camera(float fov, float width, float height) : this(1, 100, fov, width, height){}
        public Camera(float width, float height) : this(1, 100, 70, width, height){}
        public Camera() : this(1, 100, 70, 1000, 1000){}

        public dVector2 Get2DVectorFrom3D(dVector3 LocalPosition){
            float FovRad = 1.0f / MathF.Tan(Fov * 0.5f / 180.0f * 3.14159f);

            Matrix ProjectionMatrix = new Matrix(new float[,]{
                {FovRad, 0, 0, 0},
                {0, FovRad, 0, 0},
                {0, 0, Far / (Far - Near), 1.0f},
                {0, 0, (-Far * Near) / (Far - Near), 0}
            });

            Matrix Point3D = new Matrix(new float[,]{{LocalPosition.X, LocalPosition.Y, LocalPosition.Z, 1}});
            Matrix ProjectionPoint = Point3D * ProjectionMatrix;

            ProjectionPoint.data[0, 0] /= ProjectionPoint.data[0, 3];
            ProjectionPoint.data[0, 1] /= ProjectionPoint.data[0, 3];
            ProjectionPoint.data[0, 2] /= ProjectionPoint.data[0, 3];

            return new dVector2(ProjectionPoint.data[0, 0] * 0.5f * MathF.Max(Width, Height) + 0.5f * Width,
                               -ProjectionPoint.data[0, 1] * 0.5f * MathF.Max(Width, Height) + 0.5f * Height);
        }

        public static bool IsPolygonVisible(dVector3 Point1, dVector3 Point2, dVector3 Point3){
            dVector3 Normal = dVector3.Normal(Point1, Point2, Point3);
            return Normal.X * Point1.X + Normal.Y * Point1.Y + Normal.Z * Point1.Z < 0.0f;
        }

        private static float Dist(dVector3 PlaneP, dVector3 PlaneN, dVector3 p){
            dVector3 n = p.Normalize();
            return dVector3.DotProduct(PlaneN, p) - dVector3.DotProduct(PlaneN, PlaneP);
        }

        public static List<d3DTriangle> TriangleClipping(dVector3 PlaneP, dVector3 PlaneN, d3DTriangle InTriangle){
            dVector3 planeN = PlaneN.Normalize();

            List<dVector3> InsidePoints = new List<dVector3>()
            {
                new dVector3 (),
                new dVector3 (),
                new dVector3 ()
            };

            List<dVector3> OutsidePoints = new List<dVector3>()
            {
                new dVector3 (),
                new dVector3 (),
                new dVector3 ()
            };


            int nInsidePointCount = 0;
            int nOutsidePointCount = 0;

            float d0 = Dist(PlaneP, planeN, InTriangle.Point1);
            float d1 = Dist(PlaneP, planeN, InTriangle.Point2);
            float d2 = Dist(PlaneP, planeN, InTriangle.Point3);

            if (d0 >= 0){InsidePoints[nInsidePointCount] = InTriangle.Point1; nInsidePointCount++;}
            else {OutsidePoints[nOutsidePointCount] = InTriangle.Point1; nOutsidePointCount++;}
            if (d1 >= 0){InsidePoints[nInsidePointCount] = InTriangle.Point2; nInsidePointCount++;}
            else {OutsidePoints[nOutsidePointCount] = InTriangle.Point2; nOutsidePointCount++;}
            if (d2 >= 0){InsidePoints[nInsidePointCount] = InTriangle.Point3; nInsidePointCount++;}
            else {OutsidePoints[nOutsidePointCount] = InTriangle.Point3; nOutsidePointCount++;}

            if (nInsidePointCount == 3){                
                d3DTriangle OutTriangle1 = InTriangle;

                return new List<d3DTriangle>{OutTriangle1};
            }

            if (nInsidePointCount == 1 && nOutsidePointCount == 2){
                d3DTriangle OutTriangle1 = new d3DTriangle();

                OutTriangle1.Point1 = InsidePoints[0];
                OutTriangle1.Color = InTriangle.Color;

                OutTriangle1.Point2 = dVector3.IntersectPlane(PlaneP, planeN, InsidePoints[0], OutsidePoints[0]);
                OutTriangle1.Point3 = dVector3.IntersectPlane(PlaneP, planeN, InsidePoints[0], OutsidePoints[1]);

                return new List<d3DTriangle>{OutTriangle1};
            }

            if (nInsidePointCount == 2 && nOutsidePointCount == 1){
                d3DTriangle OutTriangle1 = new d3DTriangle();
                d3DTriangle OutTriangle2 = new d3DTriangle();

                OutTriangle1.Color = InTriangle.Color;
                OutTriangle2.Color = InTriangle.Color;

                OutTriangle1.Point1 = InsidePoints[0];
                OutTriangle1.Point2 = InsidePoints[1];
                OutTriangle1.Point3 = dVector3.IntersectPlane(PlaneP, planeN, InsidePoints[0], OutsidePoints[0]);

                OutTriangle2.Point1 = InsidePoints[1];
                OutTriangle2.Point2 = OutTriangle1.Point3;
                OutTriangle2.Point3 = dVector3.IntersectPlane(PlaneP, planeN, InsidePoints[1], OutsidePoints[0]);

                return new List<d3DTriangle>{OutTriangle1, OutTriangle2};
            }

            return new List<d3DTriangle>();
        }
    }
}
