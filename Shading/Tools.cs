namespace Shading{
    public static class Tools{
        public struct dColor 
        {
            public byte R {get; set;}
            public byte G {get; set;}
            public byte B {get; set;}

            public dColor(byte r, byte g, Byte b){
                R = r;
                G = g;
                B = b;
            }

            public dColor() : this(0, 0, 0){}

            public override string ToString()
            {
                return $"({R}, {G}, {B})";
            }

            public static dColor operator + (dColor c1, dColor c2){
                return new dColor((byte)(c1.R + c2.R), (byte)(c1.G + c2.G), (byte)(c1.B + c2.B));
            }

            public static dColor operator * (dColor c1, float Scalar){
                return new dColor((byte)(c1.R * Scalar), (byte)(c1.G * Scalar), (byte)(c1.B * Scalar));
            }

            public static bool operator == (dColor c1, dColor c2)
            {
                return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
            }

            public static bool operator != (dColor c1, dColor c2)
            {
                return !(c1 == c2);
            }

            public override bool Equals(object? obj)
            {
                return obj is dColor c && this == c;
            }

            public override int GetHashCode()
            {
                return R * 65536 + G * 256 + B; 
            }
        }
    }
    
}
