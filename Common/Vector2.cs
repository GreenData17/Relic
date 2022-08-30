﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Relic
{
    public class Vector2
    {
        public float X, Y;

        public Vector2() { SetVector(0, 0); }
        public Vector2(float X, float Y) { SetVector(X, Y); }
        public Vector2(float XY) { SetVector(XY, XY); }
        private void SetVector(float X, float Y) { this.X = X; this.Y = Y; }

        // operations

        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.X + b.X, a.Y + b.Y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.X - b.X, a.Y - b.Y); }
        public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.X * b.X, a.Y * b.Y); }
        public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.X / b.X, a.Y / b.Y); }
    }
}
