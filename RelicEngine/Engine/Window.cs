using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Relic.DataTypes;
using Relic.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Bitmap = System.Drawing.Bitmap;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;

namespace Relic.Engine
{
    public class Window : GameWindow
    {
        // Search for [INDEX] to jump between Sections.
        // Between sections Keep at least tree lines Empty.
        //
        // 1. [INDEX] Setup
        // 2. [INDEX] Resize Update
        // 3. [INDEX] Render Update
        // 4. [INDEX] Logic Update
        // 5. [INDEX] ImGui
        // 6. [INDEX] Unloading


        // Window Variables
        public static Window instance;
        private ImGuiController _controller;

        // Editor Specific Variables
        public static OrthographicCamera mainCam;
        public static Setting setting;
        public static string currentHoveredWindow;
        public static int loadedTextures;
        public static int loadedGameobjects;

        // Special Gui Windows
        public static bool settingIsOpen;
        public static GuiSettings guiSettings;
        public static bool debugMenuIsOpen;
        private GuiDebuggingWindow debugMenu;

        // Game Specific Variables
        public Scene currentScene;


        // Default Variables
        public static Shader defaultShader;
        public static Bitmap noTextureBitmap;
        public static string executionPath = "";

        // Lists
        public static List<Gui> gui = new();

        // GameObject
        public GameObject selectedGameObject
        {
            get => _selectedGameObject;
            set { _selectedGameObject = value; SelectedGameObjectChanged(); }
        }
        private GameObject _selectedGameObject;

        // Events
        public EventHandler OnSelectedGameObjectChanged;
        private void SelectedGameObjectChanged()
        {
            EventHandler handler = OnSelectedGameObjectChanged;
            handler?.Invoke(this, null);
        }



        // [INDEX] Setup

        public Window() : base(GameWindowSettings.Default, new() { Size = new(1920, 1010), APIVersion = new(3, 3) })
        { executionPath = Assembly.GetExecutingAssembly().Location; executionPath = executionPath.Remove(executionPath.Length-10,10); }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            SetupGlfw();
            SetupWindow();
            SetupEnvironmentAndTransparency();
            SetupCamera();
            SetupDefaultShader();
            SetupDefaultTexture();
            SetupStyles();
            SetupGui();
            setting = new(); // TODO: Create it while loading the save state

            // TODO Temp
            currentScene = new Scene();
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

#if DEBUG
            string _releaseState = "Debug";
#elif RELEASE
            string _releaseState = "Release";
#endif

            Title = "Relic: OpenGL Version: " + GL.GetString(StringName.Version) + $" -{_releaseState}-";
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            //windowSize = new Vector2(ClientSize.X, ClientSize.Y); windowSize = new Vector2(ClientSize.X, ClientSize.Y);
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
            mainCam = new OrthographicCamera();
            mainCam.CreateBufferTexture();
        }

        private void SetupDefaultShader() => defaultShader = new($"{executionPath}/Shaders/shader.vert", $"{executionPath}/Shaders/shader.frag");

        private void SetupDefaultTexture()
        {
            noTextureBitmap = new Bitmap(2, 2);
            noTextureBitmap.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 255));
            noTextureBitmap.SetPixel(0, 1, Color.FromArgb(255, 0, 0, 0));
            noTextureBitmap.SetPixel(1, 1, Color.FromArgb(255, 255, 0, 255));
            noTextureBitmap.SetPixel(1, 0, Color.FromArgb(255, 0, 0, 0));
        }

        private void SetupStyles()
        {
            ImGui.StyleColorsDark();

            ImGui.PushStyleColor(ImGuiCol.TitleBgActive , new Color(0f   , 0f   , 0f   , 1f));
            ImGui.PushStyleColor(ImGuiCol.Tab           , new Color(.3f  , .3f  , .3f  , 1f));
            ImGui.PushStyleColor(ImGuiCol.FrameBg       , new Color(.26f , .59f , .98f , .4f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Color(.492f, .725f, 1f   , .6f));

            Debug.LogCustom("Styles Loaded!", "ImGui ", new Color(.6f, 0, .5f, 1));
        }

        private void SetupGui()
        {
            InstantiateGui(new GuiInspector());
            InstantiateGui(new GuiSceneHierarchy());
            InstantiateGui(new GuiViewPort());
            InstantiateGui(new GuiFileExplorer());
            InstantiateGui(new GuiConsole());
            guiSettings = new GuiSettings();
            debugMenu = new GuiDebuggingWindow();
        }



        // [INDEX] Resize Update

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            //UpdateWindowSize();
            Size = new Vector2i(1920, 1080);
        }

        //====================

        private void UpdateWindowSize()
        {
            //windowSize = new Vector2(ClientSize.X, ClientSize.Y);
            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }



        // [INDEX] Render Update

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            WindowClearUpdates((float)e.Time);
            ImGuiUpdates();
            ViewportClearUpdates();
            if(currentScene != null)
                try{currentScene.GraphicsUpdate();}catch{}
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

        
        private bool imGuiMenu = false;

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

            if(debugMenuIsOpen) debugMenu.OnGui();
            if(settingIsOpen) guiSettings.OnGui();
            if(imGuiMenu) ImGui.ShowDemoWindow();

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Settings")) settingIsOpen = !settingIsOpen;
                    if (ImGui.MenuItem("Close")) Close();

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Not Working")) Debug.Log("What did you expect? Õ.õ");

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Debug Menu")) debugMenuIsOpen = !debugMenuIsOpen;
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
            if(mainCam is null) return;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)mainCam.bufferTexture.frameBufferName);
            GL.ClearColor(0.5f, 1f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }



        // [INDEX] Logic Update

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if(currentScene != null)
                try { currentScene.Update(); } catch { }
        }



        // [INDEX] ImGui

        public static void InstantiateGui(Gui gui)
        {
            Window.gui.Add(gui);
        }



        // ImGui Internal

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



        // [INDEX] Unloading

        public void Quit() => Close();

        protected override void OnUnload()
        {
            SaveCurrentScene();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (var gameObject in currentScene.gameObjects)
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

        public void SaveCurrentScene()
        {
            SaveManager.CreateDirectory(@"Assets\Scenes");
            SaveManager.WriteJsonFile<Scene>(currentScene, @"Assets\Scenes", $"{currentScene.name}.scene");
        }
    }
}
