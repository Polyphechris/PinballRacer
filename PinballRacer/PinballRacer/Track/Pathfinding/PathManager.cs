using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Pathfinding
{
    class PathManager
    {
        public const int HEIGHT = 15;
        public const int WIDTH = 15;
        public Dictionary<int, Obstacle> obstacles;
        public int[,] tiles;
        
        TrackGraph TileGraph;
        TrackGraph NodeGraph;
        Random random;

        public PathManager(Dictionary<int, Obstacle> o, int[,] t)
        {
            obstacles = o;
            tiles = t;
            BuildTileGraph();
        }

        public void Update(float time, Player player)
        {
            //Reseting a player's path once goal is reached or collision is found
            if (player.NullPath())
            {
                TileGraph.searchDone = false;

                TileGraph.SetStart(new Vector2(player.position.X, player.position.Y));
                int randX = (int)TileGraph.Start.position.X;
                int randY = (int)TileGraph.Start.position.Y;
                TileGraph.ResetGoal(new Vector2(randX, randY));

                while (TileGraph.End.position.X == TileGraph.Start.position.X &&
                    TileGraph.End.position.Y == TileGraph.Start.position.Y)
                {
                    //Reaquire a new goal
                    randX = random.Next(0, WIDTH - 1);
                    randY = random.Next(0, HEIGHT - 1);
                    TileGraph.ResetGoal(new Vector2(randX, randY));
                }

                //Do the A*
                TileGraph.ComputeHeuristics();
                Path path = null;

                    path = TileGraph.AStarPath();
                   // path = NodeGraph.AStarPath();

                if (path != null)
                {
                   // player.done = false;
                   // player.aStar = false;
                    if (path.points.Count > 1)
                        player.SetPath(path);
                    //else
                       // player.done = true;
                }
            }
        }

        public void Draw(Matrix view, Matrix projection, Model node, Model edge)
        {
            //TileGraph.Draw(view, projection, node, edge);
        }

        private void BuildTileGraph()
        {
            TileGraph = new TrackGraph(new Vector2(1, 1), new Vector2(1, 1), tiles.GetLength(0), (int)tiles.GetLength(1));            
            TileGraph.InitializeGrid(tiles);            
           // TileGraph.SetStart(new Vector2(new Vector2(0, 0), new Vector2(0,0));
        }

        private void BuildNodeGraph()
        {
            NodeGraph = new TrackGraph(new Vector2(1, 1), new Vector2(1, 1), tiles.GetLength(0), (int)tiles.GetLength(1));
            NodeGraph.InitializeNodes();
            // NodeGraph.SetStart(new Vector2((int)player.position.X, (int)player.position.Y));
            //NodeGraph.AddNode(NodeGraph.Start, 1, 5);
        }
    }
}
