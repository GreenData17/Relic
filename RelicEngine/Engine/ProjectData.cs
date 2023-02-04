using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relic.Engine
{

    // TODO: save and load Data
    public class ProjectData
    {
        // TODO: Set name on project creation.
        public string projectName = "New Project";
        // TODO: OnStart open this scene!
        public string lastOpenScene = "Assets\\Scenes\\New Scene.scene";

        public ProjectData()
        {
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
