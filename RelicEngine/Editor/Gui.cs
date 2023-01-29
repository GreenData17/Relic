using ImGuiNET;
using Relic.DataTypes;
using Relic.Engine;
using System;
using System.Numerics;

namespace Relic.Editor
{
    public class Gui
    {
        // Search for [INDEX] to jump between Sections.
        // Between sections Keep at least tree lines Empty.
        //
        // 1. [INDEX] ImGui Decorations
        // 2. [INDEX] ImGui Input
        // 3. [INDEX] ImGui Styling
        // 4. [INDEX] ImGui System

        public string windowName;
        public bool customImGuiStart;

        public enum Condition
        {
            None = 0,
            Always = 1,
            Once = 2,
            FirstUseEver = 4,
            Appearing = 8,
        }

        public enum ImGuiStyleVar
        {
            Alpha = 0,
            DisabledAlpha = 1,
            WindowPadding = 2,
            WindowRounding = 3,
            WindowBorderSize = 4,
            WindowMinSize = 5,
            WindowTitleAlign = 6,
            ChildRounding = 7,
            ChildBorderSize = 8,
            PopupRounding = 9,
            PopupBorderSize = 10,
            FramePadding = 11,
            FrameRounding = 12,
            FrameBorderSize = 13,
            ItemSpacing = 14,
            ItemInnerSpacing = 15,
            IndentSpacing = 16,
            CellPadding = 17,
            ScrollbarSize = 18,
            ScrollbarRounding = 19,
            GrabMinSize = 20,
            GrabRounding = 21,
            TabRounding = 22,
            ButtonTextAlign = 23,
            SelectableTextAlign = 24,
            COUNT = 25,
        }

        [Flags]
        public enum ImGuiSelectableFlags
        {
            None = 0,
            DontClosePopups = 1,
            SpanAllColumns = 2,
            AllowDoubleClick = 4,
            Disabled = 8,
            AllowItemOverlap = 16,
        }

        public Gui(string windowName, bool CustomStart = false)
        {
            this.windowName = windowName;
            customImGuiStart = CustomStart;
        }

        public void UpdateGui()
        {
            if (customImGuiStart)
            {
                OnGui();
                return;
            }

            ImGui.Begin(windowName);

            DetectHovering();

            OnGui();
            ImGui.End();
        }

        public virtual void OnGui()
        {
        }

        public void DetectHovering()
        {
            if (ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
            {
                Window.currentHoveredWindow = windowName;
            }
        }

        // [INDEX] ImGui Decorations

        public static void Label(string label) => ImGui.Text(label);

        public static void SolidLabel(string label, Vector2 size, Color color, bool black = false)
        {
            Vector2 vec = new Vector2(size.X, size.Y);

            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
            if(black) ImGui.PushStyleColor(ImGuiCol.Text, new Color(0,0,0,1));
            ImGui.Button(label, vec);
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            if (black) ImGui.PopStyleColor();
        }

        public static void Image(IntPtr ptr, Vector2 size, Vector2 uv0,
            Vector2 uv1) => ImGui.Image(ptr, size, uv0, uv1);

        // [INDEX] ImGui Interactions

        public static bool Button(string label, Vector2 size = null)
        {
            if (size is null)
                size = new Vector2(60, 20);

            Vector2 vec = new Vector2(size.X, size.Y);

            return ImGui.Button(label, vec);
        }

        public static bool SolidButton(string label, Vector2 size, Color color)
        {
            if (size is null)
                size = new Vector2(60, 20);

            Vector2 vec = new Vector2(size.X, size.Y);

            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
            bool result = ImGui.Button(label, vec);
            ImGui.PopStyleColor();
            return result;
        }

        public static bool Selectable(string label, ref bool selected, ImGuiSelectableFlags flags, Vector2 size) => 
            ImGui.Selectable(label, selected, (ImGuiNET.ImGuiSelectableFlags)flags, size);

        public static bool ComboBox(string label, ref int currentItem, string[] items)
        {
            return ImGui.Combo(label, ref currentItem, items, items.Length);
        }

        public static bool CheckBox(string label, ref bool state) => ImGui.Checkbox(label, ref state);

        // Menu's

        public static bool BeginMenuBar() => ImGui.BeginMenuBar();
        public static void EndMenuBar() => ImGui.EndMenuBar();
        public static bool BeginMenu(string label) => ImGui.BeginMenu(label);
        public static void EndMenu() => ImGui.EndMenu();
        public static bool MenuItem(string label) => ImGui.MenuItem(label);

        // TreeNode's

        public static bool TreeNode(string label, ImGuiTreeNodeFlags flags) => ImGui.TreeNodeEx(label, flags);

        // [INDEX] ImGui Input

        // int
        public static void DragInt(string label, ref int value)
        {
            ImGui.DragInt(label, ref value);
        }

        // float
        public static void DragFloat(string label, ref float value, float iteration = 0.1f, float minValue = float.MinValue,
            float maxValue = float.MaxValue, string format = "")
        {
            if (format == "")
                ImGui.DragFloat(label, ref value, iteration, minValue, maxValue);
            else
                ImGui.DragFloat(label, ref value, iteration, minValue, maxValue, format);
        }

        public static void SliderFloat(string label, ref float value, float minValue = float.MinValue,
            float maxValue = float.MaxValue, string format = "")
        {
            if (format == "")
                ImGui.SliderFloat(label, ref value, minValue, maxValue);
            else
                ImGui.SliderFloat(label, ref value, minValue, maxValue, format);
        }

        public static void InputFloat(string label, ref float value, string format = "")
        {
            if (format == "")
                ImGui.InputFloat(label, ref value);
            else
                ImGui.InputFloat(label, ref value, 0f, 0f, format);
        }

        // Text
        public static void InputText(string label, ref string content, uint maxLength = UInt32.MaxValue)
        {
            ImGui.InputText(label, ref content, maxLength);
        }

        public static void InputTextMultiline(string label, ref string content, Vector2 size, uint maxLength = UInt32.MaxValue)
        {
            Vector2 vec = new Vector2(size.X, size.Y);

            ImGui.InputTextMultiline(label, ref content, maxLength, vec);
        }

        // Color
        public static void InputColor(string label, ref Vector4 color)
        {
            ImGui.ColorEdit4(label, ref color, ImGuiColorEditFlags.NoInputs);
        }

        public static void ColorButton(string label, ref Vector4 color, Vector2 size)
        {
            if (ImGui.ColorButton(label, color, ImGuiColorEditFlags.NoInputs, size))
            {
                ImGui.OpenPopup("hi_picker");
            }

            if (ImGui.BeginPopup("hi_picker"))
            {
                ImGui.ColorPicker4("##picker", ref color);
                ImGui.EndPopup();
            }
        }

        // [INDEX] ImGui Styling

        public static void SetStyleVar(ImGuiStyleVar var, float value) => ImGui.PushStyleVar((ImGuiNET.ImGuiStyleVar)var, value);
        public static void SetStyleVar(ImGuiStyleVar var, Vector2 value) => ImGui.PushStyleVar((ImGuiNET.ImGuiStyleVar)var, value);
        public static void RemoveStyleVar() => ImGui.PopStyleVar();

        public static void SetStyleColor(ImGuiCol obj, Color color) => ImGui.PushStyleColor(obj, color);
        public static void RemoveStyleColor() => ImGui.PopStyleColor();

        public static void SameLine(float offsetFromStart = 0f, float spacing = 0f) => ImGui.SameLine(offsetFromStart, spacing);
        public static void NewLine() => ImGui.NewLine();

        public static void Separator() => ImGui.Separator();
        public static void Space(float height = 10) => ImGui.Dummy(new Vector2(0f, height));
        public static Vector2 GetWindowSize() => ImGui.GetWindowSize();

        // [INDEX] ImGui System

        // Get
        public static ImGuiIOPtr GetIO() => ImGui.GetIO();
        public static Vector2 GetMousePos() => ImGui.GetMousePos();
        public static Vector2 GetClientSize() => new Vector2(Window.instance.ClientSize.X, Window.instance.ClientSize.Y);
        public static Vector2 GetContentRegionAvail() => ImGui.GetContentRegionAvail();

        // Set

        public static void SetNextWindowPos(Vector2 position, Condition condition) =>
            ImGui.SetNextWindowPos(new Vector2(position.X, position.Y), (ImGuiCond)condition);
        public static void SetWindowPos(Vector2 position) => ImGui.SetWindowPos(new Vector2(position.X, position.Y));
        public static void SetWindowSize(Vector2 size) => ImGui.SetWindowSize(new Vector2(size.X, size.Y));
        public void SetDockSpace() =>
            ImGui.DockSpace(ImGui.GetID(windowName), Vector2.Zero(), ImGuiDockNodeFlags.None);
        
        // open

        public static void ShowDemoWindow(ref bool open) => ImGui.ShowDemoWindow(ref open);

        // ImGui start/end

        public static void Begin(string label) => ImGui.Begin(label);
        public static void Begin(string label, ImGuiWindowFlags flags) => ImGui.Begin(label, flags);
        public static void Begin(string label, ref bool open) => ImGui.Begin(label, ref open);
        public static void End() => ImGui.End();

        // child begin/end

        public static void BeginChild(string label) => ImGui.BeginChild(label);
        public static void BeginChild(string label, Vector2 size, bool border = false, ImGuiWindowFlags flags = ImGuiWindowFlags.None) => 
            ImGui.BeginChild(label, size, border, flags);

        public static void EndChild() => ImGui.EndChild();
    }
}
