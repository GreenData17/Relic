using OpenTK.Graphics.OpenGL4;
using Relic.Engine;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

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

            // Create Bitmap
            var bitmap = new Bitmap(_bitmap);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Image img = bitmap;

            // Fix Pixel format
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

            // Generate OpenGL Texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Do the work for the GC
            data = null;
            bitmap.Dispose();
            Window.loadedTextures += 1;

            return new Texture(handle);
        }

        public static Texture LoadFromResource(String resourcePath)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Bitmap bmp = new Bitmap(myStream);

            return Texture.LoadFromBitmap(bmp);
        }

        public static void OutputResources()
        {
            Assembly myAssembly1 = Assembly.GetExecutingAssembly();
            string[] names = myAssembly1.GetManifestResourceNames();
            Debug.LogEngine("========== Resources ==========");
            foreach (string name in names)
            {
                Debug.LogEngine(name);
            }
            Debug.LogEngine("========== Resources ==========");
        }

        public Texture(int glHandle)
        {
            handle = glHandle;
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(handle);
            Window.loadedTextures -= 1;
        }
    }
}
