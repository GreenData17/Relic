using System;
using System.Collections.Generic;
using System.Numerics;
using Relic.DataTypes;

namespace Relic.Engine
{
    public class Debug
    {
        public enum LogType
        {
            Log, Warning, Error, Engine, Custom
        }

        // colors
        private static Color logColor = new Color(1, 1, 1, 1);
        private static Color warningColor = new Color(1, .8f, 0, 1);
        private static Color errorColor = new Color(1, 0, 0, 1);
        private static Color engineColor = new Color(0, .5f, .8f, 1);

        // a list of all logs since last ClearConsole();
        public static List<Messages> logs { get{return _logs;} private set{_logs = value;}}
        private static List<Messages> _logs = new List<Messages>();

        // All log type calls
        public static void Log(string msg) { SendToConsole(msg, "LOG   ", ConsoleColor.White); logs.Add(new Messages(msg, LogType.Log, logColor));}
        public static void LogWarning(string msg) { SendToConsole(msg, "WARN  ", ConsoleColor.Yellow); logs.Add(new Messages(msg, LogType.Warning, warningColor)); }
        public static void LogError(string msg) { SendToConsole(msg, "ERROR ", ConsoleColor.Red); logs.Add(new Messages(msg, LogType.Error, errorColor)); }
        public static void LogEngine(string msg) { SendToConsole(msg, "ENGINE", ConsoleColor.Cyan); logs.Add(new Messages(msg, LogType.Engine, engineColor)); }
        public static void LogCustom(string msg, string prefix, Color color) { SendToConsole(msg, prefix, ConsoleColor.Cyan); logs.Add(new Messages(msg, LogType.Custom, color, prefix)); }

        

        private static void SendToConsole(string msg, string prefix, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write($" {prefix} ");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($" {msg}");
        }

        public static void ClearConsole()
        {
            logs.Clear();
        }

        public class Messages
        {
            public string msg;
            public string prefix;
            public LogType type;
            public Color color;
            public DateTime time;

            public Messages(string msg, LogType type, Color color, string prefix = "")
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
