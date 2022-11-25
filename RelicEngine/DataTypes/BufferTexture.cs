using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Relic.Engine;

namespace Relic.DataTypes
{
    public class BufferTexture
    {
        public int frameBufferName
        {
            get { return _frameBufferName; }
            private set { _frameBufferName = value; }
        }
        public int renderedBuffer
        {
            get { return _renderedBuffer; }
            private set { _renderedBuffer = value; }
        }

        private int _frameBufferName;
        private int _renderedBuffer;

        public BufferTexture()
        {
            _frameBufferName = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferName);

            GL.GenTextures(1, out _renderedBuffer);
            GL.BindTexture(TextureTarget.Texture2D, renderedBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1920, 1080, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderedBuffer, 0);
            DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Window.instance.Close();
            else
                Debug.LogEngine("Framebuffer Complete!");
        }

        public void Update()
        {
            int x = (int)MathF.Round(Window.instance.viewportSize.X);
            int y = (int)MathF.Round(Window.instance.viewportSize.Y);

            int renderedTexture;
            GL.GenTextures(1, out renderedTexture);
            GL.BindTexture(TextureTarget.Texture2D, renderedTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, x, y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);


            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, renderedTexture, 0);
            DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Window.instance.Close();
        }
    }
}
