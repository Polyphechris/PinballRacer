﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Walls
{
    class WallRegular : Wall
    {
        public WallRegular(float x, float y, Model m)
        {
            model = m;
            scale = new Vector3(0.5f, 1, 0.5f);
            position = new Vector3(x, 0, y);
        }
    }
}