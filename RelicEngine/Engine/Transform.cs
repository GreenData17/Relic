using System;

namespace Relic.Engine
{
    public class Transform : MonoBehaviour
    {
        public Vector2 position
        {
            get => _position;
            set { _position = value; PositionChanged(); }
        }
        private Vector2 _position;

        public float X, Y;
        public float rotation;

        // Events
        public EventHandler OnPositionChanged;
        private void PositionChanged()
        {
            EventHandler handler = OnPositionChanged;
            handler?.Invoke(this, null);
        }

        public Transform()
        {
            position = new Vector2(0);
            rotation = 0f;
        }

        public override void Start()
        {
            position.X = X; position.Y = Y;
        }

        public override void Update()
        {
            X = position.X; Y = position.Y;
        }
    }
}
