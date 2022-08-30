using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relic
{
    public class Camera
    {
        public Vector2 position;
        public Matrix4 view;

        public float zoom = 0.0f;

        public Camera()
        {
            position = new Vector2();
            Update();
        }

        public void Update()
        {
            view = Matrix4.CreateTranslation(Window.width / 2, Window.height / 2, -3.0f);
            view = view * Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        }
    }
}
