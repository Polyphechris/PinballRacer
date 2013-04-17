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
        public const int WAYPOINT_RADIUS = 10;
        const float STEP_TIME = 200f;
        public Dictionary<int, Obstacle> obstacles;
        public int[,] tiles;
        public List<Vector2> Waypoints;        
        
        TrackGraph TileGraph;
        TrackGraph NodeGraph;
        Random random;
        public bool aStar;
        float timer;
        public PathManager(Dictionary<int, Obstacle> o, int[,] t, List<Vector2> w)
        {
            timer = 0;
            aStar = true;
            Waypoints = w;
            obstacles = o;
            tiles = t;
            BuildTileGraph();
        }

        //Not use anymore
        private void SetWaypoint(Player p)
        {
            int quadNum;
            //Top Left inner wall
            if (p.position.X > 9 && p.position.X < RaceTrack.TRACK_WIDTH_OUT &&
                p.position.Y > 65 && p.position.Y < RaceTrack.TRACK_HEIGHT)
            {
                quadNum = 2;
            }
             //Mid Left
            else if (p.position.X > RaceTrack.TRACK_WIDTH_IN && p.position.X < RaceTrack.TRACK_WIDTH/2 - 2 &&
                p.position.Y > RaceTrack.TRACK_HEIGHT_IN && p.position.Y < RaceTrack.TRACK_HEIGHT)
            {
                quadNum = 2;
            }
            //Mid Right
            else if (p.position.X > RaceTrack.TRACK_WIDTH/2 - 2 && p.position.X < RaceTrack.TRACK_WIDTH_OUT &&
                    p.position.Y > RaceTrack.TRACK_HEIGHT_IN && p.position.Y < RaceTrack.TRACK_HEIGHT)
            {
                quadNum = 1;
            }
            //Top Mid
            else if (p.position.X > RaceTrack.TRACK_WIDTH_IN && p.position.X < RaceTrack.TRACK_WIDTH &&
                p.position.Y > RaceTrack.TRACK_HEIGHT_OUT && p.position.Y < RaceTrack.TRACK_HEIGHT)
            {
                quadNum = 1;
            }
            //Top Mid inner
            else if (p.position.X > RaceTrack.TRACK_WIDTH_IN && p.position.X < RaceTrack.TRACK_WIDTH_OUT &&
                        p.position.Y > RaceTrack.TRACK_HEIGHT_OUT - 7 && p.position.Y < RaceTrack.TRACK_HEIGHT_OUT)
            {
                quadNum = 1;
            }
            else if (p.position.X > RaceTrack.TRACK_WIDTH_OUT &&
                p.position.Y < RaceTrack.TRACK_HEIGHT_OUT)
            {
                quadNum = 0;
            }
            else if (p.position.X > RaceTrack.TRACK_WIDTH_IN &&
                     p.position.Y >= RaceTrack.TRACK_HEIGHT_OUT)
            {
                quadNum = 2;
            }
            else if (p.position.X <= RaceTrack.TRACK_WIDTH_IN &&
                     p.position.Y > RaceTrack.TRACK_HEIGHT_IN)
            {
                quadNum = 3;
            }
            else
            {
                quadNum = 4;
            }

            p.currentWaypoint = quadNum;
        }

        public void AdjustWaypoint(Player player)
        {
            float distance = Vector2.Distance(new Vector2(player.position.X, player.position.Y), Waypoints[player.currentWaypoint]);
            if (distance < WAYPOINT_RADIUS)
            {
                player.SetPath(null);
                player.currentWaypoint = (player.currentWaypoint + 1) % Waypoints.Count;
                if (player.currentWaypoint == 0)
                {
                    player.currentLap++;
                }
            }
        }

        public void Update(float time, Player player)
        {
            AdjustWaypoint(player);
            //Reseting a player's path once goal is reached or collision is found
            if (player.NullPath() && player.ImpulseCount() == 0)
            {
                TileGraph.searchDone = false;

                TileGraph.SetStart(new Vector2(player.position.X, player.position.Y));
                //Get the correct goal(S)
                //Aquire the waypoint
               // SetWaypoint(player);
                TileGraph.ResetGoal(Waypoints[player.currentWaypoint]);

                //Do the A*
                TileGraph.ComputeHeuristics(player);
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
           // TileGraph.Draw(view, projection, node, edge);
        }

        private void BuildTileGraph()
        {
            TileGraph = new TrackGraph(new Vector2(1, 1), new Vector2(40, 15), tiles.GetLength(0), (int)tiles.GetLength(1));

            TileGraph.SetStart(new Vector2(1, 1));
            TileGraph.ResetGoal(Waypoints[0]);
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
