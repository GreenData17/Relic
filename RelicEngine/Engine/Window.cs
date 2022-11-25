using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using Relic.DataTypes;
using Relic.Editor;
using Relic.Engine.UI;
using Debug = Relic.Engine.Debug;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;

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
        { Size = new Vector2i(1600, 900), APIVersion = new Version(3, 3) })
        { }

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


            // TODO: all object initialization should happen outside of this call
            //var text = new Text("start")
            //{
            //    text = "[START]",
            //    position = new Vector2(-280, 0)
            //};

            //var hello = new Text("Hello")
            //{
            //    text = "Testing Game Title",
            //    position = new Vector2(-280, 100)
            //};

            var test = Instantiate(new GameObject());
            test.name = "Start Button";
            test.transform.position = new Vector2(-280, 0);
            test.AddComponent(new Text(){text = "[START]" });

            selectedGameObject = test;

            Debug.Log("Hello World!");

            InstantiateGui(new GuiInspector());
            InstantiateGui(new GuiSceneHierarchy());
            InstantiateGui(new GuiConsole());
            //InstantiateGui(new GuiDockSpace());
            //InstantiateGui(new GuiDebuggingWindow());

            scriptsList.Add(new Text());

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

        private void SetupDefaultShader() => defaultShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

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
            UpdateWindowSize();
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

        private void ImGuiUpdates()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
            ViewPort();
            Explorer();

            if (gui.Count >= 1)
            {
                foreach (var item in gui)
                {
                    item.UpdateGui();
                }
            }

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


        // TODO: Put All ImGui stuff in separate script

        private unsafe void ViewPort()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            ImGui.Begin("Viewport");
            ImGui.PopStyleVar();
            if (viewportSize.X != ImGui.GetContentRegionAvail().X || viewportSize.Y != ImGui.GetContentRegionAvail().Y)
            {
                viewportSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y);
                mainCam.UpdateBuffer();
                //debug.Debug.Log($"New Size w:{viewportSize.X} h:{viewportSize.Y}");
            }

            ImGui.Image((IntPtr)mainCam.bufferTexture.frameBufferName + ImGuiTextureOffset, new System.Numerics.Vector2(viewportSize.X, viewportSize.Y), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            ImGui.End();
        }

        private bool selected;
        private unsafe void Explorer()
        {
            ImGui.Begin("File Explorer");

            ImGui.Button("Home");
            ImGui.SameLine();
            ImGui.Button("<");
            ImGui.NewLine();
            ImGui.Text("HOME:\\Scene\\");
            ImGui.Selectable("SampleScene", ref selected, ImGuiSelectableFlags.SpanAllColumns);

            ImGui.End();
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

            // TODO add objects unloading

            Debug.LogEngine("Engine Closing...");
            ImGui.DestroyContext();
            base.OnUnload();
        }
    }
}
