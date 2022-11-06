namespace Relic.Editor
{
    internal class GuiDebuggingWindow : Gui
    {
        public GuiDebuggingWindow() : base("Test") { }


        private float _f;
        private bool _showImGuiDemoWindow;
        private bool _showAnotherWindow;
        private int _counter;
        private int _dragInt;
        public override void OnGui()
        {
            Label("Hello, world!");                                        // Display some text (you can use a format string too)
            SliderFloat("float", ref _f, -100, 100, _f.ToString("0.000"));  // Edit 1 float using a slider from 0.0f to 1.0f    
            //ImGui.ColorEdit3("clear color", ref _clearColor);                   // Edit 3 floats representing a color

            Label($"Mouse position: {GetMousePos()}");

            CheckBox("ImGui Demo Window", ref _showImGuiDemoWindow);                 // Edit bools storing our windows open/close state
            CheckBox("Another Window", ref _showAnotherWindow);
            if (Button("Button"))                                         // Buttons return true when clicked (NB: most widgets return true when edited/activated)
                _counter++;
            SameLine(0, -1);
            Label($"counter = {_counter}");

            DragInt("Draggable Int", ref _dragInt);

            float framerate = GetIO().Framerate;
            Label($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");


            if (_showAnotherWindow)
            {
                Begin("Another Window", ref _showAnotherWindow);
                Label("Hello from another window!");
                if (Button("Close Me"))
                    _showAnotherWindow = false;
                End();
            }

            if (_showImGuiDemoWindow)
            {
                // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                // Here we just want to make the demo initial state a bit more friendly!
                SetNextWindowPos(new Vector2(650, 20), Condition.FirstUseEver);
                ShowDemoWindow(ref _showImGuiDemoWindow);
            }
        }
    }
}
