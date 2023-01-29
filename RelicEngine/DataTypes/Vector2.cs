namespace Relic
{
    public class Vector2
    {
        public float X;
        public float Y;

        public Vector2() { X = 0; Y = 0; }
        public Vector2(float X, float Y) { this.X = X; this.Y = Y; }
        public Vector2(float XY) { X = XY; Y = XY; }

        // statics

        public static Vector2 Zero() => new Vector2(0, 0);

        // operations

        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.X + b.X, a.Y + b.Y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.X - b.X, a.Y - b.Y); }
        public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.X * b.X, a.Y * b.Y); }
        public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.X / b.X, a.Y / b.Y); }

        // conversions

        public static implicit operator Vector2(OpenTK.Mathematics.Vector2 vec2) =>
            new Vector2(vec2.X, vec2.Y);

        public static implicit operator Vector2(System.Numerics.Vector2 vec2) =>
            new Vector2(vec2.X, vec2.Y);

        public static implicit operator OpenTK.Mathematics.Vector2(Vector2 vec2) =>
            new OpenTK.Mathematics.Vector2(vec2.X, vec2.Y);

        public static implicit operator System.Numerics.Vector2(Vector2 vec) =>
            new System.Numerics.Vector2(vec.X, vec.Y);

        //public static explicit operator OpenTK.Mathematics.Vector2(Vector2 vec) =>
        //    new OpenTK.Mathematics.Vector2(vec.X, vec.Y);

        //public static explicit operator System.Numerics.Vector2(Vector2 vec) =>
        //    new System.Numerics.Vector2(vec.X, vec.Y);
    }
}
