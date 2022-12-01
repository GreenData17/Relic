using OpenTK.Mathematics;
using Relic.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using static System.Formats.Asn1.AsnWriter;

namespace Relic.Engine
{
    public class Sprite : MonoBehaviour
    {
        public uint order = 0;
        public Vector2 size;
        public float scale = 1f;

        private const float BASE_SCALE = 100f;

        public Texture texture;
        public Texture texture2 = null;
        public Color overlayColor = Color.White;

        #region shader Variables

        private readonly float[] _vertices =
        { //  Position             Texture coordinates
            0.5f,  0.5f, 0.0f,   1.0f, 1.0f,
            0.5f, -0.5f, 0.0f,   1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,   0.0f, 1.0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private Shader _shader;

        private int _elementBufferObject;

        #endregion

        public Sprite()
        {
            texture = Texture.LoadFromBitmap(Window.noTextureBitmap);
        }

        public override void Start()
        {
            size = new Vector2(100);
        }

        public override void Update()
        {
            if (!_finishedInit) return;
            texture.Use(TextureUnit.Texture0);
            texture2?.Use(TextureUnit.Texture1);

            _shader.Use();

            var tempScale = BASE_SCALE * (scale / 100f); // + Window.mainCam.zoom * 10;

            var model = Matrix4.Identity;
            model = model * Matrix4.CreateScale(size.X * tempScale, size.Y * tempScale, model.ExtractScale().Z);
            model = model * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(gameObject.transform.rotation));
            model = model * Matrix4.CreateTranslation(gameObject.transform.position.X, gameObject.transform.position.Y, 0);


            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", Window.view);
            _shader.SetMatrix4("projection", Window.projection);
            _shader.SetVector4("overlayColor", Shader.ColorToVector4(overlayColor));

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void Load()
        {
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StreamDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StreamDraw);

            _shader = Window.defaultShader;
            _shader.Use();

            // Texture

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            texture ??= Texture.LoadFromBitmap(Window.noTextureBitmap);

            texture.Use(TextureUnit.Texture0);


            texture2?.Use(TextureUnit.Texture1);


            _shader.SetInt("texture0", 0);


            if (texture2 != null)
                _shader.SetInt("texture1", 1);

            _shader.SetVector4("overlayColor", Shader.ColorToVector4(overlayColor));
        }
        public override void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.handle);
        }
    }
}
