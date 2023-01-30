using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class GameObject
    {
        public bool enabled = true;
        public string name;
        public int tag;

        public Transform transform;
        [JsonIgnore]
        public List<MonoBehaviour> components;

        public GameObject(bool loading = false)
        {
            name = "New GameObject";
            tag = 0;
            components = new List<MonoBehaviour>();

            if(!loading)
                transform = AddComponent(new Transform()) as Transform;
        }

        public void Update()
        {
            foreach (var components in components)
            {
                if (components.enabled)
                    components.Update();
            }
        }

        public void GraphicsUpdate()
        {
            foreach (var components in components)
            {
                if(components.enabled)
                    components.GraphicsUpdate();
            }
        }

        public List<MonoBehaviour> SetComponents(List<MonoBehaviour> comp) => components = comp;
        public List<MonoBehaviour> GetComponents() => components;

        public MonoBehaviour GetComponent<T>()
        {
            foreach (var component in components)
            {
                if (component.GetType() == typeof(T))
                {
                    return component;
                }
            }
            return null;
        }

        public MonoBehaviour AddComponent(MonoBehaviour component)
        {
            components.Add(component);
            component.SetParent(this);
            component.Start();
            component.Load();
            component._finishedInit = true;
            return component;
        }

        public void RemoveComponent(MonoBehaviour component)
        {
            components.Remove(component);
        }

        public void LoadTransform(Transform newTransform)
        {
            foreach (var component in components)
            {
                if (component as Transform == transform)
                {
                    RemoveComponent(component);
                }

            }
            
            transform = AddComponent(newTransform) as Transform;
        }
    }
}
