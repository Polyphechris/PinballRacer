using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Pathfinding
{
    public class PathManager
    {
        public const int HEIGHT = 15;
        public const int WIDTH = 15;
        public Dictionary<int, Obstacle> obstacles;
        public int[,] tiles;
        public List<Vector2> Waypoints;        
        
        TrackGraph TileGraph;
        TrackGraph NodeGraph;
        Random random;

        public PathManager(Dictionary<int, Obstacle> o, int[,] t, List<Vector2> w)
        {
            Waypoints = w;
            obstacles = o;
            tiles = t;
            BuildTileGraph();
        }

        private void SetWaypoint(Player p)
        {
            int quadNum;

            if (p.position.X > RaceTrack.TRACK_WIDTH_OUT &&
                p.position.Y < RaceTrack.TRACK_HEIGHT_OUT)
            {
                quadNum = 0;
            }
            else if (p.position.X > RaceTrack.TRACK_WIDTH_IN &&
                     p.position.Y >= RaceTrack.TRACK_HEIGHT_OUT)
            {
                quadNum = 1;
            }
            else if (p.position.X <= RaceTrack.TRACK_WIDTH_IN &&
                     p.position.Y > RaceTrack.TRACK_HEIGHT_IN)
            {
                quadNum = 2;
            }
            else
            {
                quadNum = 3;
            }

            p.currentWaypoint = quadNum;
        }

        public void Update(float time, Player player)
        {
            //Reseting a player's path once goal is reached or collision is found
            if (player.NullPath())
            {                
                TileGraph.searchDone = false;

                TileGraph.SetStart(new Vector2(player.position.X, player.position.Y));
                //Get the correct goal(S)
                //Aquire the waypoint
                SetWaypoint(player);
                TileGraph.ResetGoal(Waypoints[player.currentWaypoint]);
                
                //Do the A*
                TileGraph.ComputeHeuristics(player);
                Path path = null;
              //  path = TileGraph.AStarPath();
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
           // TileGraph.Draw(view, projection, node, edge);
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
