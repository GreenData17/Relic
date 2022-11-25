using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Relic.Engine;
using Relic.Engine.UI;

namespace Relic.Editor
{
    public class GuiSceneHierarchy : Gui
    {
        ImGuiTreeNodeFlags childTreeNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow |
                                               ImGuiTreeNodeFlags.SpanFullWidth |
                                               ImGuiTreeNodeFlags.NoTreePushOnOpen | 
                                               ImGuiTreeNodeFlags.Leaf;

        ImGuiTreeNodeFlags parentTreeNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | 
                                                 ImGuiTreeNodeFlags.SpanFullWidth;

        public GuiSceneHierarchy() : base(Window.instance.SceneName) { customImGuiStart = true;}

        public override void OnGui()
        {
            ImGui.Begin(Window.instance.SceneName, ImGuiWindowFlags.MenuBar);

            if (BeginMenuBar())
            {
                if (BeginMenu("Add"))
                {
                    if (MenuItem("GameObject"))
                    {
                        Window.Instantiate(new GameObject());
                    }
                    if (MenuItem("Text"))
                    {
                        var obj = Window.Instantiate(new GameObject());
                        obj.name = "New Text";
                        obj.AddComponent(new Text() { text = "New Text" });
                    }

                    EndMenu();
                }

                EndMenuBar();
            }


            // TODO: Add child/parent view
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, ImGui.GetFontSize() * 1);
            foreach (GameObject gameObject in Window.gameObjects)
            {
                if (Window.instance.selectedGameObject == gameObject)
                {
                    TreeNode(gameObject.name, childTreeNodeFlags | ImGuiTreeNodeFlags.Selected);
                }
                else
                {
                    TreeNode(gameObject.name, childTreeNodeFlags);

                    if (ImGui.IsItemClicked())
                    {
                        Window.instance.selectedGameObject = gameObject;
                    }
                }
            }
            ImGui.PopStyleVar();
            
            ImGui.End();
        }
    }
}
