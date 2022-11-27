using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class Debug
    {
        public enum LogType
        {
            Log, Warning, Error, Engine
        }

        public static List<Messages> logs { get{return _logs;} private set{_logs = value;}}
        private static List<Messages> _logs = new List<Messages>();

        public static void Log(string msg) { SendToConsole(msg, "LOG   ", ConsoleColor.White); logs.Add(new Messages(msg, LogType.Log));}
        public static void LogWarning(string msg) { SendToConsole(msg, "WARN  ", ConsoleColor.Yellow); logs.Add(new Messages(msg, LogType.Warning)); }
        public static void LogError(string msg) { SendToConsole(msg, "ERROR ", ConsoleColor.Red); logs.Add(new Messages(msg, LogType.Error)); }
        public static void LogEngine(string msg) { SendToConsole(msg, "ENGINE", ConsoleColor.Cyan); logs.Add(new Messages(msg, LogType.Engine)); }

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
            public LogType type;
            public DateTime time;

            public Messages(string msg, LogType type)
            {
                this.msg = msg;
                this.type = type;
                this.time = DateTime.UtcNow;
            }
        }
    }
}
