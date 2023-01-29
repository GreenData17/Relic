using Relic.Engine;
using System.Numerics;
using Relic.DataTypes;

namespace Relic.Editor
{
    public class GuiConsole : Gui
    {
        private bool showEngineLogs;
        private bool showTime = true;

        public GuiConsole() : base("Console") { }

        public override void OnGui()
        {
            if(Button("Clear", new Vector2(50, 20)))
                Debug.logs.Clear();
            SameLine();
            Label(" Internal Logs ");
            SameLine();
            CheckBox("##ShowInternalLog", ref showEngineLogs);
            SameLine();
            Label(" Show Time ");
            SameLine();
            CheckBox("##ShowTime", ref showTime);
            Separator();

            SetStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0,2));

            for (int i = Debug.logs.Count; i > 0; i--)
            {
                if (Debug.logs[i-1].type == Debug.LogType.Engine && !showEngineLogs) continue;

                SolidLabel(" ", new Vector2(GetWindowSize().X, 20), new Color(.2f, .2f, .2f));
                SameLine(10);

                SetStyleVar(ImGuiStyleVar.FrameRounding, 12);
                if (Debug.logs[i - 1].type == Debug.LogType.Log)
                {
                    SolidLabel("LOG", new Vector2(60, 20), Debug.logs[i - 1].color, true);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Error)
                {
                    SolidLabel("ERROR", new Vector2(60, 20), Debug.logs[i - 1].color);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Warning)
                {
                    SolidLabel("WARNING", new Vector2(60, 20), Debug.logs[i - 1].color, true);
                }else if (Debug.logs[i - 1].type == Debug.LogType.Custom)
                {
                    Color color = Debug.logs[i - 1].color;
                    SolidLabel(Debug.logs[i - 1].prefix.ToUpper(), new Vector2(60, 20), color, !(color.r + color.g + color.b / 3f < .8f));
                }else if (Debug.logs[i - 1].type == Debug.LogType.Engine)
                {
                    SolidLabel("ENGINE", new Vector2(60, 20), Debug.logs[i - 1].color);
                }
                RemoveStyleVar();
                
                
                SameLine(75);
                Label($"{Debug.logs[i-1].msg}");

                if (showTime)
                {
                    SameLine(GetWindowSize().X - 90);
                    Label($"[{Debug.logs[i - 1].time:HH:mm:ss}]");
                }

            }

            RemoveStyleVar();
        }
    }
}
