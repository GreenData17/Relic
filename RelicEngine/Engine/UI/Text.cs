using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Relic.DataTypes;
using Relic.Editor;
using System;
using System.Drawing;
using System.Drawing.Text;

namespace Relic.Engine.UI
{
    public class Text : MonoBehaviour
    {
        public uint order = 0;
        public Vector2 size { get; private set; }
        public float scale = 1f;

        private const float BASE_SCALE = 100f;
        //public TextRenderingHint textRenderingHint = TextRenderingHint.SingleBitPerPixel;

        public string fontName = "Consolas";
        public Font font { get => _font; set { _font = value; UpdateText(); } }
        private Font _font = new Font("Consolas", 24f, FontStyle.Regular);
        public float fontSize = 24;
        private float _fontSize = 24;
        public bool bold { get => _bold; set {_bold = value; UpdateText();}}
        public bool _bold = false;
        public bool italic { get => _italic; set { _italic = value; UpdateText(); } }
        public bool _italic = false;
        public float r, g, b = 0;
        public float a = 1;
        public Relic.DataTypes.Color color = System.Drawing.Color.Black;
        private System.Numerics.Vector4 vColor = new System.Numerics.Vector4(0, 0, 0, 1);
        public string text = "";
        private string _text = "";
        private Texture texture = null;


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


        public override void Start()
        {
            size = new Vector2(1);
            vColor = new DataTypes.Color(r, g, b, a);
            color = new DataTypes.Color(r, g, b, a);
        }

        public override void Update()
        {
            if (Window.mainCam is null) return;
            if (text != _text)
            {
                _text = text;
                UpdateText();
            }
            if (fontSize != _fontSize)
            {
                _fontSize = fontSize;
                font = new Font(fontName, fontSize, FontStyle.Regular);
            }
        }

        public override void GraphicsUpdate()
        {
            if (Window.mainCam is null) return;
            if (!_finishedInit) return;

            texture.Use(TextureUnit.Texture0);

            _shader.Use();

            // Update OpenGL Transform
            var tempScale = BASE_SCALE * (scale / 100f);

            var model = Matrix4.Identity;
            model *= Matrix4.CreateScale(size.X * tempScale, size.Y * tempScale, model.ExtractScale().Z);
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(gameObject.transform.rotation));
            model *= Matrix4.CreateTranslation(gameObject.transform.position.X, gameObject.transform.position.Y, 0);


            // IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            // They are then transposed properly when passed to the shader. 
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", Window.mainCam.view);
            _shader.SetMatrix4("projection", Window.mainCam.projection);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void UpdateText()
        {
            if (string.IsNullOrEmpty(text)) return;

            if (bold)
                _font = new Font(fontName, _fontSize, FontStyle.Bold);
            else if(italic)
                _font = new Font(fontName, _fontSize, FontStyle.Italic);
            else
                _font = new Font(fontName, _fontSize, FontStyle.Regular);

            texture.Dispose();
            texture = null;

            SizeF stringSize;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                stringSize = g.MeasureString(text, font);
            }

            TextRenderingHint textRenderingHint;
            if (Window.setting.antiAliasing) textRenderingHint = TextRenderingHint.AntiAlias;
            else textRenderingHint = TextRenderingHint.SingleBitPerPixel;

            using (var bitmap = new Bitmap(Convert.ToInt32(stringSize.Width), Convert.ToInt32(stringSize.Height), System.Drawing.Imaging.PixelFormat.Format16bppRgb555))
            {
                bitmap.MakeTransparent();
                var gfx = Graphics.FromImage(bitmap);
                gfx.TextRenderingHint = textRenderingHint;
                gfx.DrawString(text, font, new SolidBrush(color), 0f, 0f);
                texture = Texture.LoadFromBitmap(bitmap);
            }
            size = new Vector2(stringSize.Width, stringSize.Height);

        }

        public static Font LoadFont(string path, string fontFamilyName, float size = 24f)
        {
            var collection = new PrivateFontCollection();
            collection.AddFontFile(path);
            var fontFamily = new FontFamily(fontFamilyName, collection);
            return new Font(fontFamily, size, FontStyle.Regular);
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


            _shader.SetInt("texture0", 0);
            _shader.SetVector4("overlayColor", new Vector4(1f, 1f, 1f, 1f));
        }
        public override void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.handle);
        }

        // GUI

        public override void OnGui()
        {
            Gui.Label("Color:");
            Gui.SameLine(60);
            Gui.ColorButton("Text color", ref vColor, new System.Numerics.Vector2(Gui.GetWindowSize().X - 180, 20));
            Gui.SameLine(200);
            if (Gui.Button("Update Color", new Vector2(100, 20)))
            {
                color = new Relic.DataTypes.Color(vColor.X, vColor.Y, vColor.Z, vColor.W);
                a = vColor.W;
                r = vColor.X;
                g = vColor.Y;
                b = vColor.Z;
                UpdateText();
            }


            Gui.Space(1);


            Gui.Label("size:");
            Gui.SameLine(60);
            Gui.InputFloat("##fontSize", ref fontSize);


            Gui.Space(1);


            Gui.Label("Style:");
            Gui.SameLine(60);

            if(bold) {
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Button, new System.Numerics.Vector4(0.23f, 0.37f, 0.58f, 1f));
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.29f, 0.47f, 0.73f, 1f));
                // ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(0f, 0f, 0f, 1f));
            }
            else
            {
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Button, new System.Numerics.Vector4(.24f, .24f, .24f, 1f));
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.39f, 0.39f, 0.39f, 1f));
            }

            if (Gui.Button("Bold", new Vector2(40, 20))) bold = !bold;
            Gui.SameLine(105);

            ImGuiNET.ImGui.PopStyleColor();
            ImGuiNET.ImGui.PopStyleColor();

            if (italic)
            {
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Button, new System.Numerics.Vector4(0.23f, 0.37f, 0.58f, 1f));
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.29f, 0.47f, 0.73f, 1f));
                // ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Text, new System.Numerics.Vector4(0f, 0f, 0f, 1f));
            }
            else
            {
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.Button, new System.Numerics.Vector4(.24f, .24f, .24f, 1f));
                ImGuiNET.ImGui.PushStyleColor(ImGuiNET.ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.39f, 0.39f, 0.39f, 1f));
            }

            if (Gui.Button("Italic", new Vector2(55, 20))) italic = !italic;

            ImGuiNET.ImGui.PopStyleColor();
            ImGuiNET.ImGui.PopStyleColor();
            // ImGuiNET.ImGui.RemoveStyleColor();


            Gui.Space(1);


            Gui.Label("Text:");
            Gui.SameLine(60);
            Gui.InputTextMultiline("##text", ref text, new Vector2(240, 60), 10_000);


            Gui.Space(1);
        }
    }
}
