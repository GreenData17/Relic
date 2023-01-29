using ImGuiNET;
using Relic.Engine;
using Relic.Engine.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
using Relic.DataTypes;

namespace Relic.Editor
{
    public class GuiInspector : Gui
    {
        public static List<MonoBehaviour> scriptsList = new();

        public GuiInspector() : base("Inspector")
        {
            scriptsList.Add(new Text());
            scriptsList.Add(new Sprite());
        }

        public override void OnGui()
        {
            if(Window.instance.selectedGameObject is null) return;

            ImGui.PushItemWidth(GetContentRegionAvail().X - 100);
            InputText("##name", ref Window.instance.selectedGameObject.name, 10_000);
            SameLine(GetContentRegionAvail().X - 80);
            Label("enabled ");
            SameLine(GetContentRegionAvail().X - 20);
            CheckBox("##Enabled ", ref Window.instance.selectedGameObject.enabled);
            Space(5);

            Label("Transform");
            Space(5);

            SolidLabel("X", new Vector2(20), new Color(0.8f, 0.1f, 0.1f, 1.0f));  
            SameLine(32);
            ImGui.PushItemWidth(GetContentRegionAvail().X);
            DragFloat("##posX", ref Window.instance.selectedGameObject.transform.position.X);
            ImGui.PopItemWidth();

            SolidLabel("Y", new Vector2(20), new Color(0.1f, 0.8f, 0.1f, 1.0f));
            SameLine(32);
            ImGui.PushItemWidth(GetContentRegionAvail().X);
            DragFloat("##posY", ref Window.instance.selectedGameObject.transform.position.Y);
            ImGui.PopItemWidth();

            SolidLabel("R", new Vector2(20), new Color(0.3f, 0.3f, 0.8f, 1.0f));
            SameLine(32);
            ImGui.PushItemWidth(GetContentRegionAvail().X);
            DragFloat("##rot", ref Window.instance.selectedGameObject.transform.rotation, format: Window.instance.selectedGameObject.transform.rotation.ToString("0.00") + "°");
            ImGui.PopItemWidth();
            Space();

            try
            {
                if (Window.instance.selectedGameObject != null)
                {
                    foreach (MonoBehaviour component in Window.instance.selectedGameObject.GetComponents())
                    {
                        component.BeginGui();
                    }
                }
            }
            catch { }

            AddComponentButton();
        }

        

        private void AddComponentButton()
        {
            if (Button("Add Component", new Vector2(GetContentRegionAvail().X, 20)))
            {
                ImGui.OpenPopup("ScriptMenu");
            }

            if (ImGui.BeginPopup("ScriptMenu"))
            {
                foreach (var monoBehaviour in scriptsList)
                {
                    if(ImGui.Button(monoBehaviour.GetType().Name, new System.Numerics.Vector2(100, 20)))
                    {
                        Window.instance.selectedGameObject.AddComponent((MonoBehaviour)Activator.CreateInstance(monoBehaviour.GetType()));
                        ImGui.CloseCurrentPopup();
                    }
                }
                ImGui.EndPopup();
            }
        }
    }
}
