using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Relic.DataTypes
{
    // A simple class meant to help create shaders.
    public class Shader
    {
        public readonly int handle;

        private readonly Dictionary<string, int> _uniformLocations;

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card.
        public Shader(string vertPath, string fragPath)
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // + The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            // + The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.


            // Load vertex shader and compile
            var shaderSource = File.ReadAllText(vertPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);

            // Load fragment shader and compile
            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);


            handle = GL.CreateProgram();
            // Attach both shaders...
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            LinkProgram(handle);


            // Detach and Delete the now useless shader program.
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);


            // get the number of active uniforms.
            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            // allocate the dictionary.
            _uniformLocations = new Dictionary<string, int>();
            // Loop over all uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform.
                var key = GL.GetActiveUniform(handle, i, out _, out _);
                // get the location.
                var location = GL.GetUniformLocation(handle, key);
                // add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            // Link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        // A wrapper function that enables the shader program.
        public void Use()
        {
            GL.UseProgram(handle);
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        // Uniform setters
        // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        // You use VBOs for vertex-related data, and uniforms for almost everything else.

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Vector4 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(handle);
            GL.Uniform4(_uniformLocations[name], data);
        }

        public static Vector4 ColorToVector4(Color color)
        {
            var vColor = Vector4.Zero;
            vColor.X = color.R / 255f;
            vColor.Y = color.G / 255f;
            vColor.Z = color.B / 255f;
            vColor.W = color.A / 255f;
            return vColor;
        }
    }
}
