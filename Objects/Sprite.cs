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
        public Vector2 position;

        private float baseScale = 100f;

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

        public Texture texture;
        public Texture texture2;

        public Sprite()
        {
            position = new Vector2();

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


            if(texture == null)
                texture = Texture.LoadFromFile("Resources/NoTexture.png");

            texture.Use(TextureUnit.Texture0);


            if (texture2 == null)
                texture2 = new Texture(1);

            texture2.Use(TextureUnit.Texture1);

            m_shader.SetInt("texture0", 0);
            m_shader.SetInt("texture1", 1);
        }

        public void Update()
        {
            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            m_shader.Use();

            var model = Matrix4.Identity;
            model = model * Matrix4.CreateTranslation(position.X / baseScale, position.Y / baseScale, 0);
            model = model * Matrix4.CreateScale(baseScale + Window.mainCam.zoom * 10);


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

        public void Unload()
        {
            GL.DeleteBuffer(m_vertexBufferObject);
            GL.DeleteVertexArray(m_vertexArrayObject);

            GL.DeleteProgram(m_shader.Handle);
        }
    }
}
