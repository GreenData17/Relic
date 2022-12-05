namespace Relic.Engine
{
    public class Camera : MonoBehaviour
    {
        private Vector2 oldPosition;

        public override void Start()
        {
            oldPosition = new(gameObject.transform.position.X, gameObject.transform.position.Y);
        }

        public override void Update()
        {
            if(oldPosition == null) return;
            if(oldPosition.X == gameObject.transform.position.X && oldPosition.Y == gameObject.transform.position.Y) return;

            var camPos = Window.mainCam.position;
            var objPos = gameObject.transform.position;

            Debug.Log($"camPos X:{camPos.X}|Y:{camPos.Y}");
            Debug.Log($"objPos X:{objPos.X}|Y:{objPos.Y}");

            Window.mainCam.position = new Vector2(Window.instance.ClientSize.X / 2f + objPos.X, Window.instance.ClientSize.Y / 2f + objPos.Y);
            //Debug.Log("Position changed!");

            oldPosition = new(gameObject.transform.position.X, gameObject.transform.position.Y);
        }
    }
}
