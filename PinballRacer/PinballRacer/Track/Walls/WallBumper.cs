using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PinballRacer.Track.Obstacles
{
    class WallBumper : Wall
    {
        public override Vector3 getResultingForce(Microsoft.Xna.Framework.Vector3 player)
        {
            return Vector3.Zero;
        }
    }
}
