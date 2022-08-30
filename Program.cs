using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Relic
{
    public class Program
    {
        static void Main(string[] args)
        {
            Debug.LogEngine("Engine Initializing...");

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Relic",
                Flags = ContextFlags.ForwardCompatible,
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                Debug.LogEngine("Engine Initialized!");
                window.Run();
            }
        }
    }
}
