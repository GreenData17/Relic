using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Relic.Engine;

namespace Relic.Editor
{
    public class GuiSettings : Gui
    {
        // 0 = Game Info
        // 1 = Graphics
        //
        private byte _menuSection = 0;

        public GuiSettings() : base("Settings", true) { }

        public override void OnGui()
        {
            ImGuiNET.ImGui.Begin("Settings", ref Window.settingIsOpen);
            DetectHovering();

            ImGuiNET.ImGui.BeginChild("menu", new System.Numerics.Vector2(150, GetContentRegionAvail().Y), true);

            if (Button("Game Info", new Vector2(GetContentRegionAvail().X, 20))) _menuSection = 0;
            if (Button("Graphics" , new Vector2(GetContentRegionAvail().X, 20))) _menuSection = 1;

            ImGuiNET.ImGui.EndChild();
            SameLine(0, 10);
            ImGuiNET.ImGui.BeginChild("menuContent");

            if (_menuSection == 0)
            {
                Title("Game Info");

                TextInput("Game name"   , ref Window.setting.gameName);
                TextInput("Game version", ref Window.setting.gameVersion);
                TextInput("Game creator", ref Window.setting.CreatorName);
            }else if (_menuSection == 1)
            {
                Title("Graphics");


                Title("Text");

                CheckBoxInput("antiAliasing", ref Window.setting.antiAliasing);
            }
            ImGuiNET.ImGui.EndChild();

            ImGuiNET.ImGui.End();
        }

        private void Title(string title)
        {
            Label(title);
            Separator();
            Space();
        }

        private void TextInput(string label, ref string content)
        {
            Label($"{label}:");
            SameLine(200);
            ImGuiNET.ImGui.PushItemWidth(200);
            InputText($"##{label}", ref content);
        }

        private bool CheckBoxInput(string label, ref bool active)
        {
            Label($"{label}:");
            SameLine(400);
            return CheckBox($"##{label}", ref active);
        }
    }
}
