using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Relic.Engine
{
    public class SaveManager
    {
        // TODO: Don't forget to change the save directory to the game in the game project! (because this uses the project directory and the game has to use his own assembly directory)

        // [INDEX] File Functions


        /// <summary>
        /// Creates a file.
        /// Input path without project path.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="filenameWithExtention"></param>
        public static void WriteFile(string content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }

            File.WriteAllText(path + @"\" + filenameWithExtention, content);
        }

        /// <summary>
        /// Creates a file.
        /// Input path without project path.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="filenameWithExtention"></param>
        public static void WriteFile(string[] content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }

            File.WriteAllLines(path + @"\" + filenameWithExtention, content);
        }

        // == JSON ==

        /// <summary>
        /// creates a file in json format.
        /// Input path without project path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="filenameWithExtention"></param>
        public static void WriteJsonFile<T>(object content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }
            
            string jsonContent = JsonSerializer.Serialize((T)content, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true, MaxDepth = 1000});
            File.WriteAllText(path + @"\" + filenameWithExtention, jsonContent);
        }

        public static object ReadJsonFile<T>(string path)
        {
            if (!File.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {GetProjectPath() + path} does not exist"); return null; }
            
            path = GetProjectPath() + path;
            var content = File.ReadAllText(path);

            return JsonSerializer.Deserialize(content, typeof(T), new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true, MaxDepth = 1000 });
        }

        // [INDEX] Directory Functions

        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(GetProjectPath() + path)) return;

            Directory.CreateDirectory(GetProjectPath() + path);
        }

        public static bool DirectoryExists(string path)
        {
            if (Directory.Exists(GetProjectPath() + path)) return true;
            return false;
        }

        public static bool FileExists(string path)
        {
            if (File.Exists(GetProjectPath() + path)) return true;
            return false;
        }

        public static string[] GetFiles(string path)
        {
            return Directory.GetFiles(GetProjectPath() + path);
        }

        // [INDEX] Get Functions

        public static string GetFullPath(string path) => Program.projectFolder + @"\" + path;
        public static string GetProjectPath() => Program.projectFolder + @"\";
        public static string GetEnginePath(string path) => path.Remove(0, Program.projectFolder.Length + 1);

        // Not useful for the Editor...
        [Obsolete]
        private static string GetGamePath()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.Location.Replace(assembly.FullName.Split(',')[0] + ".dll", "");
        }
    }
}
