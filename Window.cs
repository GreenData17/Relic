using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using Relic.Common;
using OpenTK.Mathematics;
using Relic.Objects;

namespace Relic
{
    public class Window : GameWindow
    {
        public static int width, height;
        
        private double m_timeSinceStart;

        public static Camera mainCam;

        // camera?
        public static Matrix4 m_view;

        public static Matrix4 m_projection;

        public static List<Sprite> sprites = new List<Sprite>();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        // This Function runs on Load.
        protected override void OnLoad()
        {
            base.OnLoad();
            width = Size.X;
            height = Size.Y;


            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // vertex
            GL.Enable(EnableCap.DepthTest);


            var sprite = new Sprite();
            sprite.texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
            sprites.Add(sprite);

            mainCam = new Camera();

            // fov is set to 45 degrees. some games also do 90 degrees.
            // m_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);
            m_projection = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, 0.0f, Size.Y, 0.1f, 1000.0f);

        }

        // This function runs on every Render frame.
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            m_timeSinceStart += 4.0 * args.Time;



            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach(Sprite s in sprites) s.Update();

            SwapBuffers();
        }

        // This function runs on every update frame.
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            HandleInput(args);
            mainCam.Update();

            base.OnUpdateFrame(args);
        }

        private void HandleInput(FrameEventArgs e)
        {
            if (!IsFocused) return;
            var input = KeyboardState;

            if (input.IsKeyReleased(Keys.Escape)) { Close(); }

            const float cameraSpeed = 150f;
            if (input.IsKeyDown(Keys.A))
                mainCam.position += new Vector2(1.0f * cameraSpeed * (float)e.Time, 0.0f);

            if (input.IsKeyDown(Keys.D))
                mainCam.position -= new Vector2(1.0f * cameraSpeed * (float)e.Time, 0.0f);

            if (input.IsKeyDown(Keys.W))
                mainCam.position -= new Vector2(0.0f, 1.0f * cameraSpeed * (float)e.Time);

            if (input.IsKeyDown(Keys.S))
                mainCam.position += new Vector2(0.0f, 1.0f * cameraSpeed * (float)e.Time);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // mainCam.zoom += e.OffsetY;

            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            //m_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);
            m_projection = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, 0.0f, Size.Y, 0.1f, 100.0f);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (Sprite s in sprites) s.Unload();

            Debug.LogEngine("Engine Closing...");
            base.OnUnload();
        }
    }
}
