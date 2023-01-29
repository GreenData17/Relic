using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relic.DataTypes
{
    public class Color
    {
        public float a, r, g, b;

        public Color(System.Drawing.Color color)
        {
            this.a = (float)color.A / 255;
            this.r = (float)color.R / 255;
            this.g = (float)color.G / 255;
            this.b = (float)color.B / 255;
        }

        public Color(int r, int g, int b, int a = 255)
        {
            this.a = (float)a / 255;
            this.r = (float)r / 255;
            this.g = (float)g / 255;
            this.b = (float)b / 255;
        }

        public Color(float r, float g, float b, float a = 1)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(float x)
        {
            r = g = b = x;
            a = x == 0 ? 0 : x;
        }
        
        // statics

        public static System.Drawing.Color FromArgb(int a, int r, int g, int b) =>
            System.Drawing.Color.FromArgb(a, r, g, b);

        public static int Color255(float a) => (int)Math.Floor(a * 255);

        // operators

        public static implicit operator System.Drawing.Color(Color color) => 
            System.Drawing.Color.FromArgb(Color255(color.a), Color255(color.r), Color255(color.g), Color255(color.b));

        public static implicit operator Vector4(Color color) => 
            new Vector4(color.r, color.g, color.b, color.a);

        public static implicit operator Color(Vector4 color) =>
            new Color(color.X, color.Y, color.Z, color.W);

        public static implicit operator Color(System.Drawing.Color color) =>
            new Color(color.R, color.G, color.B, color.A);

    }
}
