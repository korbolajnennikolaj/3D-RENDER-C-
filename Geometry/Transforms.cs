using System.Numerics;
using System.Runtime.Intrinsics;
using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace Geometry{
    public class Matrix
    {
        public float[,] data;
        public int rows;
        public int cols;

        public Matrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            data = new float[cols, rows];
        }

        public Matrix(float[,] data)
        {
            this.rows = data.GetLength(1);
            this.cols = data.GetLength(0);
            this.data = data;
        }

        public static Matrix operator + (Matrix m1, Matrix m2)
        {
            m1.CheckDimensions(m2);
            Matrix result = new Matrix(m1.rows, m1.cols);
            for (int i = 0; i < m1.cols; i++)
            {
                for (int j = 0; j < m1.rows; j++)
                {
                    result.data[i, j] = m1.data[i, j] + m2.data[i, j];
                }
            }
            return result;
        }

        public static Matrix operator - (Matrix m1, Matrix m2)
        {
            m1.CheckDimensions(m2);
            Matrix result = new Matrix(m1.rows, m1.cols);
            for (int i = 0; i < m1.cols; i++)
            {
                for (int j = 0; j < m1.rows; j++)
                {
                    result.data[i, j] = m1.data[i, j] - m2.data[i, j];
                }
            }
            return result;
        }

        public static Matrix operator * (Matrix m1, Matrix m2)
        {
            if (m1.rows != m2.cols)
            {
                throw new ArgumentException("Matrices are not compatible for multiplication.");
            }
            Matrix result = new Matrix(m1.rows, m1.cols);
            
            for (int i = 0; i < m1.cols; i++)
            {
                for (int j = 0; j < m1.rows; j++)
                {
                    for (int k = 0; k < m2.cols; k++)
                    {
                        result.data[i, j] += m1.data[i, k] * m2.data[k, j];
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    s += data[i, j].ToString("F2") + "\t";
                }
                s += "\n";
            }
            return s;
        }

        private void CheckDimensions(Matrix other)
        {
            if (rows != other.rows || cols != other.cols)
            {
                throw new ArgumentException("The size of the matrices do not match.");
            }
        }
    }


    public class dVector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public dVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public dVector2() : this(0, 0) { }

        public static dVector2 operator + (dVector2 v1, dVector2 v2)
        {
            return new dVector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static dVector2 operator - (dVector2 v1, dVector2 v2)
        {
            return new dVector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static dVector2 operator * (dVector2 v, float scalar)
        {
            return new dVector2(v.X * scalar, v.Y * scalar);
        }

        public static dVector2 operator * (float scalar, dVector2 v)
        {
            return v * scalar;
        }

        public static dVector2 operator * (dVector2 v1, dVector2 v2){
            return new dVector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static dVector2 operator / (dVector2 v, float scalar)
        {
            if (scalar != 0)
            {
                return new dVector2(v.X / scalar, v.Y / scalar);
            }
            return new dVector2(0, 0);
        }

        public static bool operator == (dVector2 v1, dVector2 v2){
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator != (dVector2 v1, dVector2 v2){
            return !(v1 == v2);
        }

        public static float DotProduct(dVector2 v1, dVector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(DotProduct(this, this));
        }

        public dVector2 Normalize()
        {
            double mag = Magnitude();
            if (mag > 0)
            {
            return this * (float)(1.0 / mag);
            }
            return new dVector2();
        }

        public double Angle()
        {
            return Math.Atan2(Y, X);
        }

        public override bool Equals(object? obj)
        {
            if (obj is dVector2 other)
            {
            return this.X == other.X && this.Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(X * 1000 + Y); 
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class dVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public dVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public dVector3(float x, float y) : this(0, 0, 0) { }

        public dVector3() : this(0, 0, 0) { }

        public static dVector3 operator + (dVector3 v1, dVector3 v2)
        {
            return new dVector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static dVector3 operator - (dVector3 v1, dVector3 v2)
        {
            return new dVector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static dVector3 operator * (dVector3 v, float scalar)
        {
            return new dVector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static dVector3 operator * (float scalar, dVector3 v)
        {
            return v * scalar;
        }

        public static dVector3 operator * (dVector3 v1, dVector3 v2){
            return new dVector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static bool operator == (dVector3 v1, dVector3 v2){
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator != (dVector3 v1, dVector3 v2){
            return !(v1 == v2);
        }

        public static float DotProduct(dVector3 v1, dVector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static dVector3 CrossProduct(dVector3 v1, dVector3 v2)
        {
            return new dVector3(
            v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X
            );
        }

        public static dVector3 Normal(dVector3 Point1, dVector3 Point2, dVector3 Point3){
            dVector3 V1 = Point2 - Point1;
            dVector3 V2 = Point3 - Point1;

            dVector3 Cross = dVector3.CrossProduct(V1, V2);

            return Cross.Normalize();
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(DotProduct(this, this));
        }

        public dVector3 Normalize()
        {
            double mag = Magnitude();
            if (mag > 0)
            {
            return this * (float)(1.0f / mag);
            }
            return new dVector3();
        }

        public static dVector3 IntersectPlane(dVector3 PlaneP, dVector3 PlaneN, dVector3 LineStart, dVector3 LineEnd){
            dVector3 planeN = PlaneN.Normalize();
            float planeD = -dVector3.DotProduct(planeN, PlaneP);
            float ad = dVector3.DotProduct(LineStart, planeN);
            float bd = dVector3.DotProduct(LineEnd, planeN);
            float t = (-planeD - ad) / (bd - ad);
            dVector3 LineStartToEnd = LineEnd - LineStart;
            dVector3 LineToIntersect = LineStartToEnd * t;
            return LineStart + LineToIntersect;
        }

        public override bool Equals(object? obj)
        {
            if (obj is dVector3 other)
            {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(X * 1000000 + Y * 1000 + Z);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    public class d3DMatrix{
        public dVector3 Origin;
        public dVector3 X;
        public dVector3 Y;
        public dVector3 Z;

        public d3DMatrix(dVector3 origin, dVector3 x, dVector3 y, dVector3 z)
        {
            Origin = origin;
            X = x;
            Y = y;
            Z = z;
        }

        public d3DMatrix(dVector3 origin) : this(origin, new dVector3(1, 0, 0), new dVector3(0, 1, 0), new dVector3(0, 0, 1)){}
        public d3DMatrix() : this(new dVector3(0, 0, 0), new dVector3(1, 0, 0), new dVector3(0, 1, 0), new dVector3(0, 0, 1)){}

        public static d3DMatrix operator + (d3DMatrix v1, dVector3 v2){
            return new d3DMatrix(v1.Origin + v2, v1.X, v1.Y, v1.Z);
        }    

        public static d3DMatrix operator - (d3DMatrix v1, dVector3 v2){
            return new d3DMatrix(v1.Origin - v2, v1.X, v1.Y, v1.Z);
        }

        public static d3DMatrix operator * (d3DMatrix v1, dVector3 v2){
            return new d3DMatrix(v1.Origin + new dVector3(dVector3.DotProduct(v1.X, v2), 
                                dVector3.DotProduct(v1.Y, v2), 
                                dVector3.DotProduct(v1.Z, v2)), 
                                v1.X, v1.Y, v1.Z);
        }

        public static d3DMatrix operator * (d3DMatrix v1, d3DMatrix v2){
            Matrix MV1 = new Matrix(new float[,]{{v1.X.X, v1.X.Y, v1.X.Z},
                                                 {v1.Y.X, v1.Y.Y, v1.Y.Z},
                                                 {v1.Z.X, v1.Z.Y, v1.Z.Z}});
            
            Matrix MV2 = new Matrix(new float[,]{{v2.X.X, v2.X.Y, v2.X.Z},
                                                 {v2.Y.X, v2.Y.Y, v2.Y.Z},
                                                 {v2.Z.X, v2.Z.Y, v2.Z.Z}});
            
            Matrix R = MV1 * MV2;

            return new d3DMatrix((v1 * v2.Origin).Origin,
                   new dVector3(R.data[0, 0], R.data[0, 1], R.data[0, 2]), 
                   new dVector3(R.data[1, 0], R.data[1, 1], R.data[1, 2]), 
                   new dVector3(R.data[2, 0], R.data[2, 1], R.data[2, 2]));
        }

        public static bool operator == (d3DMatrix v1, d3DMatrix v2){
            return v1.Origin == v2.Origin && v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator != (d3DMatrix v1, d3DMatrix v2){
            return !(v1==v2);
        }

        public dVector3 ToOrientationDegrees(){
            return new dVector3();
        }

        public dVector3 GetLocalPosition(dVector3 Position){
            dVector3 Diff = Position - Origin;
            d3DMatrix InverseMatrix = new d3DMatrix(Diff, X, Y, Z).Inverse();

            return InverseMatrix.Origin * -1.0f;
        }

        public d3DMatrix Inverse(){
            d3DMatrix InvertedMatrix = new d3DMatrix();

            InvertedMatrix.X = new dVector3(X.X, Y.X, Z.X);
            InvertedMatrix.Y = new dVector3(X.Y, Y.Y, Z.Y);
            InvertedMatrix.Z = new dVector3(X.Z, Y.Z, Z.Z);
            
            InvertedMatrix.Origin = new dVector3(
                -dVector3.DotProduct(Origin, new dVector3(InvertedMatrix.X.X, InvertedMatrix.Y.X, InvertedMatrix.Z.X)),
                -dVector3.DotProduct(Origin, new dVector3(InvertedMatrix.X.Y, InvertedMatrix.Y.Y, InvertedMatrix.Z.Y)),
                -dVector3.DotProduct(Origin, new dVector3(InvertedMatrix.X.Z, InvertedMatrix.Y.Z, InvertedMatrix.Z.Z))
            );

            return InvertedMatrix;
        }

        public static d3DMatrix Angles(dVector3 Degrees){
            float XRad = Single.DegreesToRadians(Degrees.X);
            float YRad = Single.DegreesToRadians(Degrees.Y);
            float ZRad = Single.DegreesToRadians(Degrees.Z);

            Matrix RX = new Matrix(new float[,] 
                                {{1, 0, 0},
                                {0, MathF.Cos(XRad), -MathF.Sin(XRad)},
                                {0, MathF.Sin(XRad), MathF.Cos(XRad)}});

            Matrix RY = new Matrix(new float[,] 
                                {{MathF.Cos(YRad), 0, MathF.Sin(YRad)},
                                {0, 1, 0},
                                {-MathF.Sin(YRad), 0, MathF.Cos(YRad)}});

            Matrix RZ = new Matrix(new float[,]
                                {{MathF.Cos(ZRad), -MathF.Sin(ZRad), 0},
                                {MathF.Sin(ZRad), MathF.Cos(ZRad), 0},
                                {0, 0, 1}});

            Matrix R = RX * (RY * RZ);

            return new d3DMatrix(new dVector3(), 
                          new dVector3(R.data[0, 0], R.data[0, 1], R.data[0, 2]), 
                          new dVector3(R.data[1, 0], R.data[1, 1], R.data[1, 2]), 
                          new dVector3(R.data[2, 0], R.data[2, 1], R.data[2, 2]));
        }

        public static d3DMatrix AnglesYXZ(dVector3 Degrees)
        {
            float XRad = Single.DegreesToRadians(Degrees.X);
            float YRad = Single.DegreesToRadians(Degrees.Y);
            float ZRad = Single.DegreesToRadians(Degrees.Z);

            Matrix RX = new Matrix(new float[,]
            {
                {1, 0, 0},
                {0, MathF.Cos(XRad), -MathF.Sin(XRad)},
                {0, MathF.Sin(XRad), MathF.Cos(XRad)}});

            Matrix RY = new Matrix(new float[,]
            {
                {MathF.Cos(YRad), 0, MathF.Sin(YRad)},
                {0, 1, 0},
                {-MathF.Sin(YRad), 0, MathF.Cos(YRad)}});

            Matrix RZ = new Matrix(new float[,]
            {
                {MathF.Cos(ZRad), -MathF.Sin(ZRad), 0},
                {MathF.Sin(ZRad), MathF.Cos(ZRad), 0},
                {0, 0, 1}});

            Matrix R = RY * (RX * RZ);

            return new d3DMatrix(new dVector3(),
                                new dVector3(R.data[0, 0], R.data[0, 1], R.data[0, 2]),
                                new dVector3(R.data[1, 0], R.data[1, 1], R.data[1, 2]),
                                new dVector3(R.data[2, 0], R.data[2, 1], R.data[2, 2]));
        }

        public override bool Equals(object? obj)
        {
            if (obj is d3DMatrix other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Disable overflow checking for better performance
            {
                int hash = 17;
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{Origin} {X} {Y} {Z}";
        }
    }
}
