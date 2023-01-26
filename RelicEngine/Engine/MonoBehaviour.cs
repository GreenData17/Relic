using System;
using Relic.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace Relic.Engine
{
    public class MonoBehaviour
    {
        [JsonIgnore]
        public GameObject gameObject { get; private set; }

        public string type;
        public bool enabled { get; private set; }
        public bool canDelete = true;
        public bool _finishedInit;

        public MonoBehaviour()
        {
            enabled = true;
            type = GetType().Name;
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        // Only use in Editor. like Graphics.
        public virtual void GraphicsUpdate()
        {

        }

        public virtual void OnGui()
        {

        }

        public virtual void Load()
        {

        }

        public virtual void Unload()
        {

        }

        public void SetActive(bool active)
        {
            enabled = active;

            if(active)
                Load();
            else
                Unload();
        }

        public void Delete()
        {
            Unload();
            gameObject.RemoveComponent(this);
        }

        public void SetParent(GameObject parent)
        {
            gameObject = parent;
        }

        public Dictionary<string, object> PrintPublicVars()
        {
            return GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty).ToDictionary(f => f.Name, f => f.GetValue(this));
        }

        private void ComponentTitle(string label)
        {
            Gui.Separator();
            Gui.Label(label);

            Gui.SameLine(Gui.GetContentRegionAvail().X - 50);
            
            if (GetType() != typeof(Camera) && !canDelete)
                if (Gui.SolidButton("Delete", new Vector2(50,20), new Vector4(0.3f, 0.3f, 0.3f, 1.0f)))
                {
                    gameObject.RemoveComponent(this);
                }


            Gui.Space(5);
        }

        public void BeginGui()
        {
            ComponentTitle(GetType().Name);
            OnGui();
        }
    }
}
