using SFML.Graphics;
using SFML.System;
using static Shading.Tools;

namespace Geometry{
    public static class Figures{
        public static Drawable Triangle(dVector2 Point1, dVector2 Point2, dVector2 Point3, dColor FillColor){
            SFML.Graphics.ConvexShape convex = new SFML.Graphics.ConvexShape(3);

            convex.SetPoint(0, new SFML.System.Vector2f(Point1.X, Point1.Y));
            convex.SetPoint(1, new SFML.System.Vector2f(Point2.X, Point2.Y));
            convex.SetPoint(2, new SFML.System.Vector2f(Point3.X, Point3.Y));

            convex.FillColor = new SFML.Graphics.Color(FillColor.R, FillColor.G, FillColor.B);

            return convex;
        }
        
        public static Drawable LineTriangle(dVector2 Point1, dVector2 Point2, dVector2 Point3){
            VertexArray Lines = new VertexArray(PrimitiveType.LineStrip);

            Lines.Append(new Vertex(new Vector2f(Point1.X, Point1.Y)));
            Lines.Append(new Vertex(new Vector2f(Point2.X, Point2.Y)));
            Lines.Append(new Vertex(new Vector2f(Point3.X, Point3.Y)));
            Lines.Append(new Vertex(new Vector2f(Point1.X, Point1.Y)));

            return Lines;
        }
    }
}
