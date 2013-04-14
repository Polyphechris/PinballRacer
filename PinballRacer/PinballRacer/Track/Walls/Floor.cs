using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Walls
{
    class Floor : Wall
    {
        public Floor(float x, float y, Model m)
        {
            isHit = false;
            model = m;
            position = new Vector3(x,  y, -1);
            scale = new Vector3(0.5f);
        }
    }
}
