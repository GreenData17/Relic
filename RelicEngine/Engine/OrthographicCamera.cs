using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK;
using Relic.DataTypes;

namespace Relic.Engine
{
    public class OrthographicCamera
    {
        private Vector2 _position = new Vector2(0);
        public Vector2 position
        {
            get => _position;
            set { _position = value; RecalculateViewMatrix(); }
        }


        private float _rotation = 0f;
        public float rotation
        {
            get => _rotation;
            set { _rotation = value; RecalculateViewMatrix(); }
        }

        public Matrix4 view;
        public Matrix4 projection;

        public BufferTexture bufferTexture;

        public OrthographicCamera(float left, float right, float bottom, float top)
        {
            position = new Vector2(Window.windowSize.X / 2f, Window.windowSize.Y / 2f);
            //view = Matrix4.CreateTranslation(position.X, position.Y, -3.0f);
            RecalculateViewMatrix();
            RecalculateProjectionMatrix(left, right, bottom, top);
        }

        public void RecalculateViewMatrix()
        {
            view = Matrix4.CreateTranslation(position.X, position.Y, -3.0f);
        }

        public void RecalculateProjectionMatrix(float left, float right, float bottom, float top)
        {
            projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 0.1f, 1000.0f);
        }

        public void UpdateBuffer()
        {
            bufferTexture.Update();
        }
    }
}
