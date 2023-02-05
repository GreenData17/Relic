using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{
    public class ProjectData
    {
        // TODO: Set name on project creation.
        public string projectName = "";
        public string lastOpenScene = "\\Assets\\Scenes\\New Scene.scene";

        public ProjectData()
        {
            if (projectName == "")
                projectName = new DirectoryInfo(SaveManager.GetProjectPath()).Name;

            Window.instance.Title = "Relic - " + projectName;
            Window.instance.currentScene.LoadGameobjects(lastOpenScene);
        }

        public static ProjectData Initialize()
        {
            foreach (var file in SaveManager.GetFiles(""))
            {
                if (file.EndsWith(".project"))
                {
                    return LoadProjectData(SaveManager.GetEnginePath(file));
                }
            }

            return new ProjectData();
        }

        public static ProjectData LoadProjectData(string fileName = "New Project")
        {
            return SaveManager.ReadJsonFile<ProjectData>("" + fileName) as ProjectData;
        }

        public void SaveProjectData()
        {
            SaveManager.WriteJsonFile<ProjectData>(this, "", $"{projectName}.project");
        }
    }
}
