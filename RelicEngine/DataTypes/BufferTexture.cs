using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Relic.Engine;
using System;

namespace Relic.DataTypes
{
    public class BufferTexture
    {
        public int frameBufferName
        {
            get => _frameBufferName;
            private set => _frameBufferName = value;
        }
        public int renderedBuffer
        {
            get => _renderedBuffer;
            private set => _renderedBuffer = value;
        }

        public Vector2i BufferSize = new(1920, 1080);

        private int _frameBufferName;
        private int _renderedBuffer;

        public BufferTexture()
        {
            // Create Buffer
            _frameBufferName = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferName);

            // Create and Bind Texture
            GL.GenTextures(1, out _renderedBuffer);
            GL.BindTexture(TextureTarget.Texture2D, renderedBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, BufferSize.X, BufferSize.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            // bind Texture to Buffer
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderedBuffer, 0);
            DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            // check if failed
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Window.instance.Close();
            else
                Debug.LogEngine("Framebuffer Complete!");
        }
    }
}
