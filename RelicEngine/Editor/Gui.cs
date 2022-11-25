using ImGuiNET;
using Relic.DataTypes;
using Relic.Engine;
using System;
using System.Numerics;

namespace Relic.Editor
{
    public class Gui
    {
        // TODO: MORE INPUT OPTIONS / MORE STUFF
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

        public Gui(string windowName)
        {
            this.windowName = windowName;
        }

        public void UpdateGui()
        {
            if (customImGuiStart)
            {
                OnGui();
                return;
            }

            ImGui.Begin(windowName);
            OnGui();
            ImGui.End();
        }

        public virtual void OnGui()
        {
        }

        //====================
        // -- Editor Calls --
        //====================

        public static void Label(string label) => ImGui.Text(label);

        public static void SolidLabel(string label, Vector2 size, Vector4 color, bool black = false)
        {
            System.Numerics.Vector2 vec = new System.Numerics.Vector2(size.X, size.Y);

            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
            if(black) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0,0,0,1));
            ImGui.Button(label, vec);
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            if (black) ImGui.PopStyleColor();
        }

        //====================
        // -- Interactions --
        //====================

        public static bool Button(string label, Vector2 size = null)
        {
            if (size is null)
                size = new Vector2(60, 20);

            System.Numerics.Vector2 vec = new System.Numerics.Vector2(size.X, size.Y);

            return ImGui.Button(label, vec);
        }

        public static bool SolidButton(string label, Vector2 size, Vector4 color)
        {
            if (size is null)
                size = new Vector2(60, 20);

            System.Numerics.Vector2 vec = new System.Numerics.Vector2(size.X, size.Y);

            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
            bool result = ImGui.Button(label, vec);
            ImGui.PopStyleColor();
            return result;
        }

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

        //====================
        // -- Inputs --
        //====================

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
            System.Numerics.Vector2 vec = new System.Numerics.Vector2(size.X, size.Y);

            ImGui.InputTextMultiline(label, ref content, maxLength, vec);
        }

        // Color
        public static void InputColor(string label, ref Vector4 color)
        {
            ImGui.ColorEdit4(label, ref color, ImGuiColorEditFlags.NoInputs);
        }

        public static void ColorButton(string label, ref Vector4 color, System.Numerics.Vector2 size)
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

        //====================
        // -- Styling --
        //====================

        public static void SetStyleColor(ImGuiCol obj, Vector4 color) => ImGui.PushStyleColor(obj, color);
        public static void PopStyleColor() => ImGui.PopStyleColor();

        public static void SameLine(float offsetFromStart = 0f, float spacing = 0f) =>
            ImGui.SameLine(offsetFromStart, spacing);

        public static void Separator() => ImGui.Separator();
        public static void Space(float height = 10) => ImGui.Dummy(new System.Numerics.Vector2(0f, height));
        public static System.Numerics.Vector2 GetWindowSize() => ImGui.GetWindowSize();

        //====================
        // -- ImGui system --
        //====================

        // Get
        public static ImGuiIOPtr GetIO() => ImGui.GetIO();
        public static System.Numerics.Vector2 GetMousePos() => ImGui.GetMousePos();
        public static Vector2 GetClientSize() => new Vector2(Window.instance.ClientSize.X, Window.instance.ClientSize.Y);

        // Set

        public static void SetNextWindowPos(Vector2 position, Condition condition) =>
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(position.X, position.Y), (ImGuiCond)condition);
        public static void SetWindowPos(Vector2 position) => ImGui.SetWindowPos(new System.Numerics.Vector2(position.X, position.Y));
        public static void SetWindowSize(Vector2 size) => ImGui.SetWindowSize(new System.Numerics.Vector2(size.X, size.Y));
        public void SetDockSpace() =>
            ImGui.DockSpace(ImGui.GetID(windowName), System.Numerics.Vector2.Zero, ImGuiDockNodeFlags.None);
        
        // open

        public static void ShowDemoWindow(ref bool open) => ImGui.ShowDemoWindow(ref open);

        // ImGui start/end

        public static void Begin(string label) => ImGui.Begin(label);
        public static void Begin(string label, ref bool open) => ImGui.Begin(label, ref open);
        public static void End() => ImGui.End();
    }
}
