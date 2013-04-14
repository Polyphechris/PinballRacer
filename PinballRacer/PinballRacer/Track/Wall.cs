using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track
{
    public abstract class Wall : Obstacle
    {
        public Vector3 start;
        public Vector3 end;
        
        public override Vector3 getResultingForce(Player p)
        {
            //throw new NotImplementedException();
            return Vector3.Zero;
        }
    }
}
