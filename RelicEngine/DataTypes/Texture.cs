using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Relic.DataTypes
{
    public class Texture
    {
        public readonly int handle;

        public static Texture LoadFromBitmap(Bitmap _bitmap)
        {
            // Generate handle
            int handle = GL.GenTexture();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            var bitmap = new Bitmap(_bitmap);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Image img = bitmap;
            byte[,,] data = new byte[bitmap.Height, bitmap.Width, 4];

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    data[y, x, 0] = bitmap.GetPixel(x, y).R;
                    data[y, x, 1] = bitmap.GetPixel(x, y).G;
                    data[y, x, 2] = bitmap.GetPixel(x, y).B;
                    data[y, x, 3] = bitmap.GetPixel(x, y).A;
                }
            }



            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            data = null;
            bitmap.Dispose();

            return new Texture(handle);
        }

        public Texture(int glHandle)
        {
            handle = glHandle;
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
