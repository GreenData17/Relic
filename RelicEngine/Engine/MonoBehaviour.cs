using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Relic.Editor;

namespace Relic.Engine
{
    public class MonoBehaviour
    {
        public object inheritor;
        public GameObject gameObject;

        public bool enabled { get; private set; }
        public bool _finishedInit;

        public MonoBehaviour()
        {
            inheritor = this;
            enabled = true;
            if (inheritor.GetType() == typeof(MonoBehaviour)) Window.instance.Close();
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Load()
        {

        }

        public virtual void Unload()
        {

        }

        public virtual void OnGui()
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

        public Dictionary<string, object> PrintPublicVars()
        {
            return inheritor.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty).ToDictionary(f => f.Name, f => f.GetValue(inheritor));
        }

        private void ComponentTitle(string label)
        {
            Gui.Separator();
            Gui.Label(label);

            Gui.SameLine(Gui.GetContentRegionAvail().X - 50);

            // TODO: Make it possible to switch between cameras! ex. Button labeled: "Make Main" or "Take Main Camera OwnerShip".
            if (GetType() != typeof(Camera))
            if (Gui.SolidButton("Delete", new Vector2(50,20), new Vector4(0.3f, 0.3f, 0.3f, 1.0f)))
            {
                gameObject.RemoveComponent(this);
            }


            Gui.Space(5);
        }

        public void BeginGui()
        {
            ComponentTitle(inheritor.GetType().Name);
            OnGui();
        }
    }
}
