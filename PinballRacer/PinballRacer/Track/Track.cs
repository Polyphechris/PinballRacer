using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track
{
    class Track
    {
        List<Obstacle> obstacles;
        List<Wall> walls;

        TrackGraph TileGraph;
        TrackGraph NodeGraph;

        public float[,] TerrainMap;

        public Track()
        {

        }
    }
}
