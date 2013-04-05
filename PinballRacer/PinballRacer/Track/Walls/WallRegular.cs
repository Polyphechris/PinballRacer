using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Walls
{
    class WallRegular : Wall
    {
        public const float E = 1f;
        public WallRegular(float x, float y, Model m)
        {
            model = m;
            scale = new Vector3(0.5f,  0.5f, 2f);
            position = new Vector3(x,  y, 0);
        }       
    }
}
