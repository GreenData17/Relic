using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Relic.DataTypes;
using static System.Net.Mime.MediaTypeNames;

namespace Relic.Engine
{
    public class Scene
    {
        public string name = "New Scene";
        public GameObject mainCamera;

        [JsonIgnore]
        public List<GameObject> gameObjects = new List<GameObject>();
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, object>> variableData = new Dictionary<string, Dictionary<string, object>>();

        //            Gameobject name   comonement name    variable name   \/----variable data
        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> gameobjectData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

        public Scene() => setup();

        public Scene(Scene scene)
        {
            name = scene.name;
            foreach (var gameObject in scene.gameObjects)
            {
                var obj = Instantiate(gameObject);

                foreach (dynamic component in obj.components)
                {
                    try
                    {
                        if (component.mainCamera)
                            mainCamera = obj;
                    }
                    catch { }

                    foreach (var script in GetAllSubclassesOf(typeof(MonoBehaviour)))
                    {

                        if (component.GetType() == script)
                        {

                        }
                    }
                }

                gameObjects.Add(obj);
            }
        }

        private void setup()
        {
            var camObj = Instantiate(new GameObject());
            camObj.name = "Main Camera";
            var camComp = new Camera();
            camComp.mainCamera = true;
            camObj.AddComponent(camComp);
            mainCamera = camObj;
            camObj = null;



            camObj = Instantiate(new GameObject());
            camObj.name = "Main Camera2";
            camComp = new Camera();
            camObj.AddComponent(camComp);
            camObj = null;
        }

        public void Update()
        {
            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.enabled) return;

                gameObject.Update();
            }
        }

        public void GraphicsUpdate()
        {
            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.enabled) return;

                gameObject.GraphicsUpdate();
            }
        }

        public GameObject Instantiate(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            Window.loadedGameobjects += 1;
            return gameObject;
        }

        public void DeleteGameObject(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
            Window.loadedGameobjects -= 1;
        }

        public static List<Type> GetAllSubclassesOf(Type baseType)
        {
            return Assembly.GetAssembly(baseType).GetTypes().Where(type => type.IsSubclassOf(baseType)).ToList();
        }
    }
}
