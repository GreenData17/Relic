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



        public ProjectData LoadProjectData()
        {
            return SaveManager.ReadJsonFile<ProjectData>("") as ProjectData;
        }

        public void SaveProjectData()
        {
            SaveManager.WriteJsonFile<ProjectData>(this, "", $"{projectName}.project");
        }
    }
}
