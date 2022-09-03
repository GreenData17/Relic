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
using Relic.UI;
using System.Threading;
using System.Drawing;
using System.Drawing.Text;

namespace Relic
{
    public class Window : GameWindow
    {
        public static int width, height;

        public static double FPS;
        private DateTime lastFPSWait;
        private double timeSinceStart;

        Random random = new Random();

        // view //

        public static Camera mainCam;
        public static Matrix4 m_projection;

        // resources //

        public static List<Sprite> sprites = new List<Sprite>();
        public static List<dynamic> UIs = new List<dynamic>();

        // game data //

        private enum GameState { titleScreen, game, resultScreen }
        private GameState gameState = GameState.titleScreen;
        private bool pause;

        // bird
        private Sprite bird;
        private bool birdDied;
        private bool jump;

        // pipes
        private DateTime lastSpawn;
        private List<Sprite> pipes = new List<Sprite>();

        // UI
        private Text gameTitle;
        private Text startHint;
        private Text text;
        private Text position;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        // This Function runs on Load.
        protected override void OnLoad()
        {
            #region default OnLoad
            base.OnLoad();
            width = Size.X;
            height = Size.Y;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 0.0f);
            //GL.Enable(EnableCap.DepthTest);  // Only 3D
            GL.DepthFunc(DepthFunction.Never); // Only 2D
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            mainCam = new Camera();
            // fov is set to 45 degrees. some games also do 90 degrees.  ▼ this MAtrix4 is only used in 3D
            // m_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);
            m_projection = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, 0.0f, Size.Y, 0.1f, 1000.0f);
            #endregion

            GL.ClearColor(0, .6f, .8f, 0);

            Start();
        }

        // This function runs on every Render frame.
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            #region renderframe start
            base.OnRenderFrame(args);
            
            timeSinceStart += 4.0 * args.Time;

            var tempLastFPSWait = new DateTime(lastFPSWait.Ticks).AddSeconds(1);

            if (tempLastFPSWait <= DateTime.Now)
            {
                lastFPSWait = DateTime.Now;
                FPS = 1f / args.Time;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit/* | ClearBufferMask.DepthBufferBit*/);

            #endregion
                Title = "Relic | FPS: " + FPS.ToString("0.00");

            // <Game Render Logic>

            Render(args);

            // </Game Render Logic>

            #region renderframe end

            for (uint i = 0; i <= 10; i++)
            {
                foreach (Sprite s in sprites)
                {
                    if (s.order == i && s.enabled)
                    {
                        s.Update();
                    }
                }
            }

            for (uint i = 0; i <= 10; i++)
            {
                foreach (dynamic d in UIs)
                {
                    if (d.order == i && d.enabled)
                    {
                        d.Update();
                    }
                }
            }

            // IMPORTANT //
            // if the all drawn textures are transparent, then it won't draw anything.
            // but if something solid is drawn afterwards, then it draws totally fine.
            // DO NOT DELETE THIS! //
            var displayPreWarmer = new Sprite("NaN");
            displayPreWarmer.scale = 0f;

            SwapBuffers();
            #endregion
        }

        // This function runs on every update frame.
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            HandleInput(args);
            mainCam.Update();

            Update(args);

            base.OnUpdateFrame(args);
        }


        // Inititializing Objects here!
        private void Start()
        {
            text = new Text("text1");
            text.text = "HEllo World! This is finnally working!!!!! XD TT (╯°□°）╯︵ ┻━┻";
            text.color = Color.Red;
            text.scale = 0.005f;
            text.SetActive(false);

            position = new Text("positionText");
            position.text = "X: -   Y: -";
            position.scale = 0.006f;
            position.position = new Vector2(-(width / 2) + 70, (height / 2) - 20);

            gameTitle = new Text("Title");
            gameTitle.text = "Flappy TK";
            gameTitle.font = loadFont("Resources/Flappybird.ttf", "FlappyBirdy", 40f);
            gameTitle.color = Color.LimeGreen;
            gameTitle.position = new Vector2(0, 100);
            gameTitle.scale = 0.03f;

            startHint = new Text("StartHint");
            startHint.text = "- Press Space to start -";
            startHint.position = new Vector2(0, -50);

            bird = new Sprite("bird");
            bird.texture = Texture.LoadFromFile("Resources/bird.png");
            bird.position = new Vector2(-300, 0);
            bird.scale = 0.6f;
            bird.order = 1;
            bird.SetActive(false);

            var temp2 = new Sprite("pos2");
            temp2.order = 8;
            temp2.size = new Vector2(.8f, .8f);
            temp2.rotation = 90f;
            temp2.SetActive(false);
        }

        // Draw your objects here. (sprite update is already handeled.)
        private void Render(FrameEventArgs args)
        {
            
        }

        // put your logic here.
        private void Update(FrameEventArgs args)
        {
            position.text = $"X: {mainCam.position.X.ToString("0.00")}, Y: {mainCam.position.Y.ToString("0.00")}";
            position.position = new Vector2(-(width / 2) + (position.size.X / 3), (height / 2) - (position.size.Y / 3));

            if (gameState == GameState.game)
            {
                if (!pause)
                {
                    BirdLogic(args);
                    PipeSpawning(args);
                    PipeLogic(args);
                }
            }
            if(gameState == GameState.resultScreen)
            {
                List<Sprite> list = new List<Sprite>(pipes);
                foreach(Sprite s in list)
                {
                    pipes.Remove(s);
                    s.Delete();
                }

                bird.SetActive(false);
            }
        }

        private void BirdLogic(FrameEventArgs args)
        {
            if (birdDied)
                bird.SetActive(false);
            else
                bird.SetActive(true);

            if (!bird.enabled) return;

            if (bird.rotation > -45f)
                bird.rotation += -80f * (float)args.Time;

            if (!jump)
                bird.position -= new Vector2(0, 300f * (float)args.Time);

            if (jump && bird.position.Y < 300)
                bird.position += new Vector2(0, 600f * (float)args.Time);

            if (bird.position.Y < -500)
            {
                birdDied = true;
                bird.SetActive(false);
                gameState = GameState.resultScreen;
            }
        }

        private void PipeSpawning(FrameEventArgs args)
        {
            var tempLastspawn = new DateTime(lastSpawn.Ticks).AddSeconds(5);

            if (pipes.Count < 4 && tempLastspawn <= DateTime.Now)
            {
                lastSpawn = DateTime.Now;
                var tempPipe = new Sprite("pipe");
                tempPipe.texture = Texture.LoadFromFile("Resources/PipeBot.png");
                tempPipe.position += new Vector2(400, -500 + (50 * random.Next(1, 8)));
                tempPipe.size = new Vector2(1.5f, 5);

                pipes.Add(tempPipe);
            }

            try
            {
                foreach (Sprite s in pipes)
                {
                    s.position -= new Vector2(100f * (float)args.Time, 0);
                    if (s.position.X < -400) { pipes.Remove(s); s.Delete(); }
                }
            }catch{ }
        }



        private void PipeLogic(FrameEventArgs args)
        {
            try
            {

                foreach (Sprite s in pipes)
                {
                    if (s.position.X < -250 && s.position.X > -350) 
                    {
                        if (bird.position.Y < s.position.Y + (s.size.Y * 100 / 2) + 20)
                        {
                            gameState = GameState.resultScreen;
                        }
                    }

                    foreach (Sprite ss in sprites)
                    {
                        if (ss.name == "pos2")
                        {
                            ss.position = new Vector2(s.position.X, s.position.Y + (s.size.Y * 100 / 2));
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        private void HandleInput(FrameEventArgs e)
        {
            if (!IsFocused) return;
            var input = KeyboardState;

            if (input.IsKeyReleased(Keys.Escape)) { Close(); }

            // title

            if (input.IsKeyReleased(Keys.Space) && gameState == GameState.titleScreen)
            {
                gameTitle.SetActive(false);
                startHint.SetActive(false);
                gameState = GameState.game;
            }

            // bird

            if (input.IsKeyDown(Keys.Space))
            {
                bird.rotation = 45f;
                jump = true;
            }
            else jump = false;

            // camera

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

        public Font loadFont(string path, string fontFamilyName, float size = 24f)
        {
            PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(path);
            FontFamily fontFamily = new FontFamily(fontFamilyName, collection);
            return new Font(fontFamily, size, FontStyle.Regular);
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

            width = Size.X;
            height = Size.Y;
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // foreach (Sprite s in sprites) s.Unload();

            Debug.LogEngine("Engine Closing...");
            base.OnUnload();
        }
    }
}
