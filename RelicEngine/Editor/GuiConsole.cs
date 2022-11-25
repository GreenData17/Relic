using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Relic.Engine;

namespace Relic.Editor
{
    public class GuiConsole : Gui
    {
        private bool showEngineLogs;

        public GuiConsole() : base("Console") { }

        public override void OnGui()
        {
            if(Button("Clear", new Vector2(50, 20)))
                Debug.logs.Clear();
            SameLine();
            Label(" Internal Logs ");
            SameLine();
            CheckBox("##ShowInternalLog", ref showEngineLogs);
            Separator();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0,0));

            for (int i = Debug.logs.Count; i > 0; i--)
            {
                if (Debug.logs[i-1].type == Debug.LogType.Engine && !showEngineLogs) continue;

                if (Debug.logs[i - 1].type == Debug.LogType.Log)
                {
                    SolidLabel("LOG", new Vector2(60, 20), new Vector4(1, 1, 1, 1), true);
                    SameLine(75);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Error)
                {
                    SolidLabel("ERROR", new Vector2(60, 20), new Vector4(1, 0, 0, 1));
                    SameLine(75);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Warning)
                {
                    SolidLabel("WARNING", new Vector2(60, 20), new Vector4(1, .8f, 0, 1), true);
                    SameLine(75);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Engine)
                {
                    SolidLabel("ENGINE", new Vector2(60, 20), new Vector4(0, .5f, .8f, 1), true);
                    SameLine(75);
                }

                // TODO: add logType Image
                Label(Debug.logs[i-1].msg);
            }

            ImGui.PopStyleVar();
        }
    }
}
