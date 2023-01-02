using Relic.Editor;

namespace Relic.Engine
{
    public class Camera : MonoBehaviour
    {
        public bool mainCamera;
        private Vector2 _oldPosition;

        public override void Start()
        {
            _oldPosition = new(gameObject.transform.position.X, gameObject.transform.position.Y);
        }

        public override void GraphicsUpdate()
        {
            if(_oldPosition == null) return;
            if(_oldPosition.X == gameObject.transform.position.X && _oldPosition.Y == gameObject.transform.position.Y) return;

            var camPos = Window.mainCam.position;
            var objPos = gameObject.transform.position;

            Debug.Log($"camPos X:{camPos.X}|Y:{camPos.Y}");
            Debug.Log($"objPos X:{objPos.X}|Y:{objPos.Y}");

            Window.mainCam.position = new Vector2(Window.instance.ClientSize.X / 2f + objPos.X, Window.instance.ClientSize.Y / 2f + objPos.Y);
            //Debug.Log("Position changed!");

            _oldPosition = new(gameObject.transform.position.X, gameObject.transform.position.Y);

            if (mainCamera) canDelete = !mainCamera;
        }

        public override void OnGui()
        {
            if(mainCamera) return;

            if(Gui.Button("Set as Main Camera", new Vector2(Gui.GetContentRegionAvail().X, 25)))
            {
                Camera oldCam = Window.instance.currentScene.mainCamera.GetComponent<Camera>() as Camera;
                oldCam.mainCamera = false;
                oldCam = null;
                Window.instance.currentScene.mainCamera = this.gameObject;
                mainCamera = true;
            }
        }
    }
}
