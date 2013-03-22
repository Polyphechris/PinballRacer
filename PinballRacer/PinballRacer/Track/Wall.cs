using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PinballRacer.Track
{
    public abstract class Wall : Obstacle
    {
        public Vector3 start;
        public Vector3 end;
        
        public override Vector3 getResultingForce(Vector3 player)
        {
            return Vector3.Zero;
        }
    }
}
