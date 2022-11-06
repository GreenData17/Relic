using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class Debug
    {
        public static void Log(string msg) { SendToConsole(msg, "LOG   ", ConsoleColor.White); }
        public static void LogWarning(string msg) { SendToConsole(msg, "WARN  ", ConsoleColor.Yellow); }
        public static void LogError(string msg) { SendToConsole(msg, "ERROR ", ConsoleColor.Red); }
        public static void LogEngine(string msg) { SendToConsole(msg, "ENGINE", ConsoleColor.Cyan); }

        //====================

        private static void SendToConsole(string msg, string prefix, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write($" {prefix} ");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($" {msg}");
        }
    }
}
