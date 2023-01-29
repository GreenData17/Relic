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
        
        private List<gameObjectData> gameObjectsDataCollection = new List<gameObjectData>();


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


        public void SaveGameobjects()
        {
            foreach (var gameObject in gameObjects)
            {
                var data = new gameObjectData();
                data.enabled = gameObject.enabled;
                data.name = gameObject.name;
                data.tag = gameObject.tag;

                var componenets = new Dictionary<string, Dictionary<string, object>>();

                foreach (var component in gameObject.components)
                {
                    var scriptFields = new Dictionary<string, object>();
                    foreach (var fieldInfo in component.GetType().GetFields())
                    {
                        scriptFields.Add(fieldInfo.Name, fieldInfo.GetValue(component));
                    }
                    componenets.Add(component.name, scriptFields);
                }

                data.componentData = componenets;
                gameObjectsDataCollection.Add(data);
            }

            SaveManager.WriteJsonFile<List<gameObjectData>>(gameObjectsDataCollection, "Assets\\Scenes", "test2.scene");
            gameObjectsDataCollection = null;
        }

        public void LoadGameobjects()
        {
            gameObjectsDataCollection = SaveManager.ReadJsonFile<List<gameObjectData>>("Assets\\Scenes\\test2.scene") as List<gameObjectData>;

            if(gameObjectsDataCollection == null ) return;
            gameObjects.Clear();

            foreach (var gameObject in gameObjectsDataCollection)
            {
                var obj = new GameObject();
                obj.enabled = gameObject.enabled;
                obj.name = gameObject.name;
                obj.tag = gameObject.tag;

                foreach (var component in gameObject.componentData)
                {
                    Type type = Type.GetType(component.Key);
                    if(type == null) continue;

                    var script = Activator.CreateInstance(type);
                    
                    foreach (var fieldData in component.Value)
                    {
                        FieldInfo fieldInfo = script.GetType().GetField(fieldData.Key);
                        if(fieldInfo == null) continue;
                        
                        try
                        {
                            fieldInfo.SetValue(script, Convert.ChangeType(fieldData.Value.ToString(), fieldInfo.FieldType));
                        }
                        catch {}


                    }

                    obj.AddComponent(script as MonoBehaviour);
                }
                
                Instantiate(obj);
                var cam = (Camera)obj.GetComponent<Camera>();
                if (cam != null && cam.mainCamera)
                {
                    mainCamera = obj;
                }
            }
        }

        public class gameObjectData
        {
            public bool enabled;
            public string name;
            public int tag;

            public Dictionary<string, Dictionary<string, object>> componentData =  new Dictionary<string, Dictionary<string, object>>();
        }
    }
}
