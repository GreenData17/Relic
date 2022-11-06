using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Debug = Relic.Engine.Debug;

namespace Relic.Editor
{
    public class GuiDockSpace : Gui
    {
        public GuiDockSpace() : base("DockSpace")
        {
            customImGuiStart = true;
        }

        private ImGuiWindowFlags flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus;

        public override void OnGui()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            ImGui.Begin("DockSpace", flags);
            ImGui.PopStyleVar();
            SetDockSpace();

            if (BeginMenuBar())
            {
                if (BeginMenu("File"))
                {
                    if (MenuItem("Open Project"))
                        Debug.LogWarning("Button not set!");

                    EndMenu();
                }
                if (BeginMenu("Edit"))
                {


                    EndMenu();
                }
                if (BeginMenu("Help"))
                {


                    EndMenu();
                }
                EndMenuBar();
            }

            SetWindowPos(new Vector2(0));
            SetWindowSize(GetClientSize());
            ImGui.End();
        }
    }
}
