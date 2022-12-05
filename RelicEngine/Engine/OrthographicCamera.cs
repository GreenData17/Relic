using OpenTK.Mathematics;
using Relic.DataTypes;

namespace Relic.Engine
{
    public class OrthographicCamera
    {
        public Vector2 position
        {
            get => _position;
            set { _position = value; RecalculateViewMatrix(); }
        }
        private Vector2 _position;

        // Camera Specific Variables
        public Matrix4 view;
        public Matrix4 projection;
        
        public BufferTexture bufferTexture;
        

        public OrthographicCamera()
        {
            position = new(Window.instance.ClientSize.X / 2f, Window.instance.ClientSize.Y / 2f);
            RecalculateViewMatrix();
            RecalculateProjectionMatrix();
        }

        public void RecalculateViewMatrix()
        {
           view = Matrix4.CreateTranslation(position.X, position.Y, -3.0f);
        }

        public void RecalculateProjectionMatrix()
        {
            projection = Matrix4.CreateOrthographicOffCenter(0, Window.instance.ClientSize.X, 0, Window.instance.ClientSize.Y, 0.1f, 1000.0f);
        }

        public void CreateBufferTexture() => bufferTexture = new BufferTexture();
    }
}
