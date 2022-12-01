using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Relic.DataTypes;
using Relic.Editor;
using Relic.Engine.UI;
using Debug = Relic.Engine.Debug;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
using System.IO;
using System.Reflection;

namespace Relic.Engine
{
    public class Window : GameWindow
    {
        // Window Variables
        private ImGuiController _controller;
        public static Window instance;
        public static Vector2 windowSize;
        public Vector2 viewportSize = new Vector2(1024, 768);
        private const bool DEBUG_MODE = false;
        
        //  TODO: TEMP
        public string SceneName = "DefaultScene";


        // Graphics Variables
        public static Shader defaultShader;
        // - Camera
        public static OrthographicCamera mainCam;
        public static Matrix4 view;
        public static Matrix4 projection;
        public static int ImGuiTextureOffset = 1;


        // Default Variables
        public static Bitmap noTextureBitmap;
        public static string executionPath = "";


        // GameObject
        public GameObject selectedGameObject
        {
            get { return _selectedGameObject; }
            set { _selectedGameObject = value; SelectedGameObjectChanged(); }
        }
        private GameObject _selectedGameObject;

        public EventHandler OnSelectedGameObjectChanged;
        // GameObject Lists
        public static List<Gui> gui = new List<Gui>();
        public static List<MonoBehaviour> scriptsList = new List<MonoBehaviour>();
        public static List<GameObject> gameObjects = new List<GameObject>();



        //====================
        // -- Setup Calls --
        //====================

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings()
            { Size = new Vector2i(1920, 1010), APIVersion = new Version(3, 3) })
        { executionPath = Assembly.GetExecutingAssembly().Location; executionPath = executionPath.Remove(executionPath.Length-10,10); Debug.Log(executionPath);}

        void SelectedGameObjectChanged()
        {
            EventHandler handler = OnSelectedGameObjectChanged;
            handler?.Invoke(this, null);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            SetupGlfw();
            SetupWindow();
            SetupEnvironmentAndTransparency();
            SetupCamera();
            SetupDefaultShader();
            SetupDefaultTexture();
            ImGui.StyleColorsDark();

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new System.Numerics.Vector4(0f,0f,0f,1f));
            ImGui.PushStyleColor(ImGuiCol.Tab, new System.Numerics.Vector4(.3f,.3f,.3f,1f));

            Debug.LogCustom("Styles Loaded!", "ImGui", new System.Numerics.Vector4(.6f,0,.5f,1));

            // TODO: all object initialization should happen outside of this call

            //Assembly myAssembly1 = Assembly.GetExecutingAssembly();
            //string[] names = myAssembly1.GetManifestResourceNames();
            //foreach (string name in names)
            //{
            //    Debug.LogEngine(name);
            //}

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.ProfileIcon-transparent.png");
            Bitmap bmp = new Bitmap(myStream);

            var test3 = Instantiate(new GameObject());
            test3.name = "ProfilPicture";
            test3.AddComponent(new Sprite() { size = new Vector2(1000), texture = Texture.LoadFromBitmap(bmp)});

            var test0 = Instantiate(new GameObject());
            test0.name = "Title Text";
            test0.transform.position = new Vector2(0, 230);
            test0.transform.rotation = 15f;
            test0.AddComponent(new Text() { text = "The Game", fontSize = 130, bold = true});

            var test1 = Instantiate(new GameObject());
            test1.name = "Start Button";
            test1.transform.position = new Vector2(0, 0);
            test1.AddComponent(new Text(){text = "[START]", fontSize = 40});

            var test2 = Instantiate(new GameObject());
            test2.name = "Exit Button";
            test2.transform.position = new Vector2(0, -80);
            test2.AddComponent(new Text() { text = "[EXIT]", fontSize = 40 });

            selectedGameObject = test1;

            Debug.Log("Hello World!");

            InstantiateGui(new GuiInspector());
            InstantiateGui(new GuiSceneHierarchy());
            InstantiateGui(new GuiViewPort());
            InstantiateGui(new GuiFileExplorer());
            InstantiateGui(new GuiConsole());

            debWin = new GuiDebuggingWindow();

            scriptsList.Add(new Text());
            scriptsList.Add(new Sprite());

        }

        //====================

        private unsafe void SetupGlfw()
        {
            if (GLFW.GetCurrentContext() == null) Debug.LogError("ERROR Context is NULL");
            GLFW.SetErrorCallback(GlfwErrorCallback);
        }

        private void GlfwErrorCallback(ErrorCode code, string msg)
        {
            Debug.LogError($"glfw error in {code}: {msg}");
        }

        private void SetupWindow()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("There can be only one MainWindow!");
                Close();
            }

            Title = "Relic: OpenGL Version: " + GL.GetString(StringName.Version);
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            windowSize = new Vector2(ClientSize.X, ClientSize.Y); windowSize = new Vector2(ClientSize.X, ClientSize.Y);
            WindowState = WindowState.Maximized;
        }

        private void SetupEnvironmentAndTransparency()
        {
            GL.ClearColor(0.5f, 0.7f, 0.5f, 1.0f);
            GL.DepthFunc(DepthFunction.Never);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            CursorState = CursorState.Hidden;
        }

        private void SetupCamera()
        {
            mainCam = new OrthographicCamera(0f, ClientSize.X, 0f, ClientSize.Y);
            mainCam.rotation = 100f;
            view = mainCam.view;
            projection = mainCam.projection;
            mainCam.bufferTexture = new BufferTexture();
        }

        private void SetupDefaultShader() => defaultShader = new Shader($"{executionPath}/Shaders/shader.vert", $"{executionPath}/Shaders/shader.frag");

        private void SetupDefaultTexture()
        {
            noTextureBitmap = new Bitmap(2, 2);
            noTextureBitmap.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 255));
            noTextureBitmap.SetPixel(0, 1, Color.FromArgb(255, 0, 0, 0));
            noTextureBitmap.SetPixel(1, 1, Color.FromArgb(255, 255, 0, 255));
            noTextureBitmap.SetPixel(1, 0, Color.FromArgb(255, 0, 0, 0));
        }

        //====================
        // -- Resize Calls --
        //====================

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            //UpdateWindowSize();
            Size = new Vector2i(1920, 1080);
        }

        //====================

        private void UpdateWindowSize()
        {
            windowSize = new Vector2(ClientSize.X, ClientSize.Y);
            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        //====================
        // -- Render Calls --
        //====================

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            WindowClearUpdates((float)e.Time);
            ImGuiUpdates();
            ViewportClearUpdates();
            DrawUpdates();
            SwapBuffers();
        }

        //====================

        private void WindowClearUpdates(float time)
        {
            _controller.Update(this, time);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(0.4f, 0.4f, 0.4f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private bool debugMenu = false;
        private bool imGuiMenu = false;
        private GuiDebuggingWindow debWin = null;

        private void ImGuiUpdates()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

            if (gui.Count >= 1)
            {
                foreach (var item in gui)
                {
                    item.UpdateGui();
                }
            }

            if(debugMenu) debWin.OnGui();
            if(imGuiMenu) ImGui.ShowDemoWindow();

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Debug Menu")) debugMenu = !debugMenu;
                    if (ImGui.MenuItem("Imgui Menu")) imGuiMenu = !imGuiMenu;

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            _controller.Render();
            ImGuiController.CheckGLError("End of frame");
        }

        private void ViewportClearUpdates()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)mainCam.bufferTexture.frameBufferName);
            GL.ClearColor(0.5f, 1f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private void DrawUpdates()
        {
            foreach (var obj in gameObjects)
            {
                if (obj.enabled)
                {
                    obj.Update();
                }
            }
        }

        //====================
        // -- Update Calls --
        //====================

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            //foreach (dynamic uI in UIs)
            //{
            //    if (uI.name == "Text")
            //    {
            //        uI.text = $"{_counter}";
            //    }
            //}
        }

        //====================
        // -- Temp. ImGui --
        //====================

        public static void InstantiateGui(Gui gui)
        {
            Window.gui.Add(gui);
        }

        public static GameObject Instantiate(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            return gameObject;
        }

        //====================
        // -- Input Calls --
        //====================

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            // Gives ImGui the keys pressed!
            _controller.PressChar((char)e.Unicode);
        }

        private float lastScrolevalue = 0f;
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _controller.MouseScroll(new Vector2(e.Offset.X, e.Offset.Y));
            lastScrolevalue = e.OffsetY;
        }

        //====================
        // -- Unloading --
        //====================

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (var gameObject in gameObjects)
            {
                foreach (var component in gameObject.components)
                {
                    component.Unload();
                }
            }

            Debug.LogEngine("Engine Closing...");
            ImGui.DestroyContext();
            base.OnUnload();
        }

        public void Quit() => Close();
    }
}
