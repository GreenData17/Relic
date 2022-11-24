using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Relic.Engine;

namespace Relic.Editor
{
    public class GuiInspector : Gui
    {

        public GuiInspector() : base("Inspector") { }

        public override void OnGui()
        {

            Label("Transform");
            Space(5);

            SolidLabel("X", new Vector2(20), new Vector4(0.8f, 0.1f, 0.1f, 1.0f));
            SameLine(32);
            DragFloat("##posX", ref Window.instance.selectedGameObject.transform.position.X);

            SolidLabel("Y", new Vector2(20), new Vector4(0.1f, 0.8f, 0.1f, 1.0f));
            SameLine(32);
            DragFloat("##posY", ref Window.instance.selectedGameObject.transform.position.Y);

            SolidLabel("R", new Vector2(20), new Vector4(0.3f, 0.3f, 0.8f, 1.0f));
            SameLine(32);
            DragFloat("##rot", ref Window.instance.selectedGameObject.transform.rotation, format: Window.instance.selectedGameObject.transform.rotation.ToString("0.00") + "°");
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

        /*
        [Obsolete]
        public void ProcessComponent()
        {
            for (int i = 0; i < components.Count; i++)
            {
                value = components[i].PrintPublicVars();


                ComponentTitle(components[i].GetType().Name);


                foreach (string valueKey in value.Keys)
                {
                    if (value[valueKey] is string)
                    {
                        string v = (string)value[valueKey];
                        Label($"{valueKey}:");
                        SameLine();
                        DrawString("", ref v);
                        value[valueKey] = v;
                    }
                    else if (value[valueKey] is Color)
                    {
                        Color c = (Color)value[valueKey];
                        Vector4 v = new Vector4(c.R, c.G, c.B, c.A);
                        Label($"{valueKey}:");
                        SameLine();
                        InputColor("", ref v);
                    }
                }

                foreach (var propertyInfo in components[i].GetType().GetProperties())
                {
                    if (components[i].GetType().GetProperty(propertyInfo.Name) != null){
                        if (components[i].GetType().GetProperty(propertyInfo.Name).GetValue(components[i]) is string)
                        {
                            var property = components[i].GetType().GetProperty(propertyInfo.Name);

                            string v = (string)components[i].GetType().GetProperty(propertyInfo.Name).GetValue(components[i]);
                            Label($"{propertyInfo.Name}:");
                            SameLine();
                            DrawString("", ref v);

                            var convertedValue = Convert.ChangeType(v, property.PropertyType);
                            property.SetValue(components[i], convertedValue);
                        }
                        else if (components[i].GetType().GetProperty(propertyInfo.Name).GetValue(components[i]) is Color)
                        {
                            var property = components[i].GetType().GetProperty(propertyInfo.Name);

                            Color c = (Color)components[i].GetType().GetProperty(propertyInfo.Name).GetValue(components[i]);
                            Vector4 v = new Vector4(c.R, c.G, c.B, c.A);
                            Label($"{propertyInfo.Name}:");
                            SameLine();
                            InputColor("", ref v);
                            c = Color.FromArgb((int)v.W / 255, (int)v.X / 255, (int)v.Y / 255, (int)v.Z / 255);

                            var convertedValue = Convert.ChangeType(c, property.PropertyType);
                            property.SetValue(components[i], convertedValue);
                        }

                    }
                }
            }

            Window.instance.selectedGameObject.SetComponents(components);
        }


        private void ComponentTitle(string label)
        {
            Separator();
            Label(label);

            SameLine(42);
            if (SolidButton("Delete", null, new Vector4(0.3f, 0.3f, 0.3f, 1.0f)))
            {
                Debug.LogWarning("Button Not Implemented!");
            }


            Space(5);
        }

        private void DrawString(string label, ref string value)
        {
            Label(label);
            SameLine();
            InputText(label, ref value, 200);
        }*/

        private void AddComponentButton()
        {
            if (Button("Add Component", new Vector2(GetWindowSize().X, 20)))
            {
                ImGui.OpenPopup("ScriptMenu");
            }

            if (ImGui.BeginPopup("ScriptMenu"))
            {
                foreach (var monoBehaviour in Window.scriptsList)
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
