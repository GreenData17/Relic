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

        public GuiSceneHierarchy() : base("Scene") { customImGuiStart = true;}

        public override void OnGui()
        {
            Begin("Scene", ImGuiWindowFlags.MenuBar);
            DetectHovering();

            Label(Window.instance.currentScene == null ? "No Scene Open" : Window.instance.currentScene.name);
            Separator();

            if (Window.instance.currentScene == null)
            {
                return;
            }

            if (BeginMenuBar())
            {
                if (BeginMenu("Add"))
                {
                    if (MenuItem("GameObject"))
                    {
                        Window.instance.currentScene.Instantiate(new GameObject());
                    }
                    if (MenuItem("Text"))
                    {
                        var obj = Window.instance.currentScene.Instantiate(new GameObject());
                        obj.name = "New Text";
                        obj.AddComponent(new Text() { text = "New Text" });
                    }
                    if (MenuItem("Sprite"))
                    {
                        var obj = Window.instance.currentScene.Instantiate(new GameObject());
                        obj.name = "New Sprite";
                        obj.AddComponent(new Sprite() { size = new Vector2(100)});
                    }

                    EndMenu();
                }

                if (MenuItem("Save"))
                {
                    // SaveManager.WriteJsonFile<Scene>(Window.instance.currentScene, @"Assets\Scenes", "default.scene");
                    Window.instance.currentScene.SaveGameobjects();
                }

                EndMenuBar();
            }
            
            // TODO: Add child/parent view
            SetStyleVar(ImGuiStyleVar.IndentSpacing, ImGui.GetFontSize() * 1);
            foreach (GameObject gameObject in Window.instance.currentScene.gameObjects)
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
            RemoveStyleVar();
            
            End();
        }
    }
}
