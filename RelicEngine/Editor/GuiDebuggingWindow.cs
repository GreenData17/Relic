using Relic.Engine;

namespace Relic.Editor
{
    internal class GuiDebuggingWindow : Gui
    {
        public GuiDebuggingWindow() : base("Test") { }
        
        public override void OnGui()
        {
            float framerate = GetIO().Framerate;
            Label($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
            Space();
            Label($"Loaded gameObjects: {Window.loadedGameobjects}");
            Label($"Loaded textures:    {Window.loadedTextures}");
        }
    }
}
