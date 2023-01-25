using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace Relic.Engine
{
    public class Setting
    {
        // Game Info
        public string gameName    = "new Game";
        public string gameVersion = "0.0.1";
        public string CreatorName = "defaultCreator";

        public Image gameIcon     = null;


        // Graphics
        public bool antiAliasing  = false;
    }
}
