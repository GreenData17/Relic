using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relic.DataTypes;

namespace Relic.Engine
{
    public class Transform
    {
        public Vector2 position;
        public float rotation;


        public Transform()
        {
            position = new Vector2(0);
            rotation = 0f;
        }
    }
}
