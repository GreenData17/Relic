using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Editor
{
    public class GuiFileExplorer : Gui
    {
        public GuiFileExplorer() : base("File Explorer") { }

        private bool selected;

        public override void OnGui()
        {
            Button("Home");
            SameLine(75);
            Button("<", new Vector2(20));
            Separator();
            Label("HOME:\\Scene\\");
            Selectable("SampleScene", ref selected, ImGuiSelectableFlags.SpanAllColumns);
        }
    }
}
