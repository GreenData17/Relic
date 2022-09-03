using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;
using Relic.Common;
using OpenTK.Mathematics;

namespace Relic.Objects
{
    public class Sprite
    {
        public bool enabled { get; private set; }

        public string name;
        public uint order = 0;
        public Vector2 position;
        public Vector2 size;
        public float rotation = 0f;
        public float scale = 1f;

        private float baseScale = 100f;

        public Texture texture = null;
        public Texture texture2 = null;

        private bool finishedInit;



        #region shader Variables

        private readonly float[] m_vertices =
        { //  Position             Texture coordinates
              0.5f,  0.5f, 0.0f,   1.0f, 1.0f,
              0.5f, -0.5f, 0.0f,   1.0f, 0.0f,
             -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
             -0.5f,  0.5f, 0.0f,   0.0f, 1.0f
        };

        private readonly uint[] m_indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int m_vertexBufferObject;
        private int m_vertexArrayObject;

        private Shader m_shader;

        private int m_elementBufferObject;

        #endregion


        public Sprite(string name)
        {
            this.name = name;
            setup();
        }

        public Sprite(string name, Texture texture)
        {
            this.name = name;
            this.texture = texture;
            setup();
        }

        private void setup()
        {
            // Debug.LogEngine($"[sprite] \"{name}\" created!");

            position = new Vector2();
            size = new Vector2(1);

            Load();

            Window.sprites.Add(this);
            enabled = true;
            finishedInit = true;
        }

        public void Update()
        {
            if (!finishedInit) return;

            texture.Use(TextureUnit.Texture0);

            if (texture2 != null)
                texture2.Use(TextureUnit.Texture1);


            m_shader.Use();

            var tempscale = (baseScale * scale) ; // + Window.mainCam.zoom * 10;

            var model = Matrix4.Identity;
            model = model * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation));
            model = model * Matrix4.CreateScale(size.X * tempscale, size.Y * tempscale, model.ExtractScale().Z);
            model = model * Matrix4.CreateTranslation(position.X, position.Y, 0);


            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            // This means that we retain the same multiplication order in both OpenTK c# code and GLSL shader code.
            // If you pass the individual matrices to the shader and multiply there, you have to do in the order "model * view * projection".
            // You can think like this: first apply the modelToWorld (aka model) matrix, then apply the worldToView (aka view) matrix, 
            // and finally apply the viewToProjectedSpace (aka projection) matrix.
            m_shader.SetMatrix4("model", model);
            m_shader.SetMatrix4("view", Window.mainCam.view);
            m_shader.SetMatrix4("projection", Window.m_projection);

            GL.DrawElements(PrimitiveType.Triangles, m_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void SetActive(bool state)
        {
            if (enabled && state) return;
            else if (!enabled && !state) return;
            else if (enabled && !state) Unload();
            else if (!enabled && state) Load();
        }

        public void Load()
        {
            if (enabled) return;
            enabled = true;

            m_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(m_vertexArrayObject);

            m_vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, m_vertices.Length * sizeof(float), m_vertices, BufferUsageHint.StaticDraw);

            m_elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, m_indices.Length * sizeof(uint), m_indices, BufferUsageHint.StaticDraw);

            m_shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            m_shader.Use();

            // Texture

            var vertexLocation = m_shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = m_shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            if (texture == null)
                texture = Texture.LoadFromFile("Resources/NoTexture.png");

            texture.Use(TextureUnit.Texture0);


            if (texture2 != null)
                texture2.Use(TextureUnit.Texture1);


            m_shader.SetInt("texture0", 0);


            if (texture2 != null)
                m_shader.SetInt("texture1", 1);
        }
        public void Unload()
        {
            if (!enabled) return;
            enabled = false;

            GL.DeleteBuffer(m_vertexBufferObject);
            GL.DeleteVertexArray(m_vertexArrayObject);

            GL.DeleteProgram(m_shader.Handle);
        }

        public void Delete()
        {
            Unload();
            Window.sprites.Remove(this);
        }
    }
}
