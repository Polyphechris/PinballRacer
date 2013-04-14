using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Players;

namespace PinballRacer.Track.Walls
{
    class WallBonus : Wall
    {
        public const float E = 1f;

        public override Vector3 getResultingForce(Player p)
        {
            //throw new NotImplementedException();
            return Vector3.Zero;
        }
    }
}
