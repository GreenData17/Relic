using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class Debug
    {
        public enum LogType
        {
            Log, Warning, Error, Engine, Custom
        }

        private static Vector4 logColor = new Vector4(1, 1, 1, 1);
        private static Vector4 warningColor = new Vector4(1, 0, 0, 1);
        private static Vector4 errorColor = new Vector4(1, 1, 1, 1);
        private static Vector4 engineColor = new Vector4(0, .5f, .8f, 1);

        public static List<Messages> logs { get{return _logs;} private set{_logs = value;}}
        private static List<Messages> _logs = new List<Messages>();

        public static void Log(string msg) { SendToConsole(msg, "LOG   ", ConsoleColor.White); logs.Add(new Messages(msg, LogType.Log, logColor));}
        public static void LogWarning(string msg) { SendToConsole(msg, "WARN  ", ConsoleColor.Yellow); logs.Add(new Messages(msg, LogType.Warning, warningColor)); }
        public static void LogError(string msg) { SendToConsole(msg, "ERROR ", ConsoleColor.Red); logs.Add(new Messages(msg, LogType.Error, errorColor)); }
        public static void LogEngine(string msg) { SendToConsole(msg, "ENGINE", ConsoleColor.Cyan); logs.Add(new Messages(msg, LogType.Engine, engineColor)); }
        public static void LogCustom(string msg, string prefix, Vector4 color) { SendToConsole(msg, prefix, ConsoleColor.Cyan); logs.Add(new Messages(msg, LogType.Custom, color, prefix)); }

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



        public class Messages
        {
            public string msg;
            public string prefix;
            public LogType type;
            public Vector4 color;
            public DateTime time;

            public Messages(string msg, LogType type, Vector4 color, string prefix = "")
            {
                this.msg = msg;
                this.type = type;
                this.time = DateTime.UtcNow;
                this.color = color;
                this.prefix = prefix;
            }
        }
    }
}
