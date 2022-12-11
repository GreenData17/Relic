using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class GameObject
    {
        public bool enabled = true;
        public string name;
        public int tag;

        public Transform transform;
        public List<MonoBehaviour> components;

        public GameObject()
        {
            name = "New GameObject";
            tag = 0;
            transform = new Transform();
            components = new List<MonoBehaviour>();
        }

        public void Update()
        {
            foreach (var components in components)
            {
                if(components.enabled)
                    components.EditorUpdate();
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

        public void AddComponent(MonoBehaviour component)
        {
            components.Add(component);
            component.SetParent(this);
            component.Start();
            component.Load();
            component._finishedInit = true;
        }

        public void RemoveComponent(MonoBehaviour component)
        {
            components.Remove(component);
        }
    }
}
