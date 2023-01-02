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
        public string gameName;
        public string gameVersion;
        public string companyName;

        public Image gameIcon;


        // Graphics
        public bool antiAliasing;
    }
}
