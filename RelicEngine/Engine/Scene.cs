using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class Scene
    {
        public string name = "New Scene";
        public GameObject mainCamera;
        public List<GameObject> gameObjects = new List<GameObject>();

        public Scene()
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
    }
}
