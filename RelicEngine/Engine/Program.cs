using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;

namespace Relic.Engine
{
    class Program
    {
        public static string projectFolder = "/Test Project";

        static void Main(string[] args)
        {
            projectFolder = Directory.GetCurrentDirectory() + "/Test Project";

            if (args.Length == 0) { args = new string[1]; }
            if (!string.IsNullOrEmpty(args[0])){ projectFolder = args[0];}

            Window wnd = new Window();
            wnd.Run();
        }
    }
}
