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

        public static void WriteFile(string content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }

            File.WriteAllText(path + @"\" + filenameWithExtention, content);
        }

        public static void WriteFile(string[] content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }

            File.WriteAllLines(path + @"\" + filenameWithExtention, content);
        }

        // json

        public static void WriteJsonFile<T>(object content, string path, string filenameWithExtention)
        {
            if (!Directory.Exists(GetProjectPath() + path))
            { Debug.LogError($"[SaveManager] {path} does not exist"); return; }

            if (Directory.Exists(GetProjectPath() + path))
            { path = GetProjectPath() + path; }
            
            string jsonContent = JsonSerializer.Serialize((T)content, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true, MaxDepth = 1000});
            File.WriteAllText(path + @"\" + filenameWithExtention, jsonContent);
        }

        // [INDEX] Directory Functions

        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(GetProjectPath() + path)) return;

            Directory.CreateDirectory(GetProjectPath() + path);
        }

        // [INDEX] Get Functions

        private static string GetProjectPath()
        {
            return Program.projectFolder + @"\";
        }

        // Not useful for the Editor...
        private static string GetGamePath()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.Location.Replace(assembly.FullName.Split(',')[0] + ".dll", "");
        }
    }
}
