﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Content;
using PinballRacer.Track;

namespace PinballRacer
{
    class PlayerCollisionManager
    {
        const float DETECTION_LENGTH = 3f;
        public Player human;
        public List<Player> NPC;        
        RaceTrack Track;
        ContentManager content;
        Model wall;
        Model ball;
        static int ranking;


        public PlayerCollisionManager(ContentManager c, RaceTrack t)
        {
            // Sets the game rank
            ranking = 1;

            Track = t;
            content = c;
            NPC = new List<Player>();
        }

        public void InitializePlayers(List<NpcPlayer> players, Player human)
        {            
            this.human = human;
            foreach (NpcPlayer npc in players)
            {                
                NPC.Add(npc);
            }
            NPC.Add(human);
        }

        public void update(GameTime time)
        {
            foreach (Player p in NPC)
            {
                if (p.doneRace)
                {
                    continue;
                }
                
                if (p != human)
                {
                    Track.PathController.Update(time.ElapsedGameTime.Milliseconds, p);
                }
                else
                {
                    Track.PathController.AdjustWaypoint(p);
                }

                //Check P2P collision
                foreach (Player p2 in NPC)
                {
                    if (p2.doneRace)
                    {
                        continue;
                    }
                    if (p2 != p)
                    {
                        if (Collision(p, p2) && Game1.enableCollisionDetection)
                        {
                            p.SetPath(null);
                            HandleCollision(p, p2);
                        }
                    }
                }

                //Only use walls that are in range
                int x = (int)p.position.X;
                int y = (int)p.position.Y;
                if (x == 0) ++x;
                if (y == 0) ++y;
                Dictionary<int, Obstacle> obstaclesInRange = new Dictionary<int, Obstacle>();
                for (int i = x - 1; i <= x + 2; ++i)
                {
                    for (int j = y - 1; j <= y + 2; ++j)
                    {
                        if (Track.tiles[i, j] != 0)
                        {
                            if (!obstaclesInRange.ContainsKey(Track.tiles[i, j]))
                                obstaclesInRange.Add(Track.tiles[i, j], Track.obstacles[Track.tiles[i, j]]);
                        }
                    }
                }

                List<Vector4> Impulses = new List<Vector4>();
                foreach (Obstacle o in obstaclesInRange.Values)
                {
                    //Checks for and returns forces
                    Vector3 i = o.getResultingForce(p);
                    if (!i.Equals(Vector3.Zero))
                    {
                        p.SetPath(null);

                        // Before applying impulses, resolve collision
                        p.position = new Vector3(p.previousPosition.X, p.previousPosition.Y, p.previousPosition.Z);

                        Impulses.Add(new Vector4(i.X, i.Y, i.Z, 0));
                    }
                }
                p.AddImpulses(Impulses);

                p.Update(time);
            }    
        }

        public void draw(Matrix view, Matrix projection)
        {
            foreach (Player p in NPC)
            {
                p.Draw(view, projection);
            }
        }
        
        public bool Collision(Player p1, Player p2)
        {
            float distance = (p1.position - p2.position).Length();
            if(distance < Player.RADIUS * 2)
            {
                AudioManager.playEffect(AudioEffect.WALL_BOUNCE);
                return true;
            }
            return false;
        }

        public void HandleCollision(Player p1, Player p2)
        {
            //Recompute a path, apply bounce forces, steer
            Vector3 ImpulseDirection = p1.position - p2.position;
            ImpulseDirection.Normalize();
            List<Vector4> newList = new List<Vector4>();
            newList.Add(new Vector4(ImpulseDirection.X, ImpulseDirection.Y, 0, 0));
            List<Vector4> newList1 = new List<Vector4>();
            newList1.Add(new Vector4(-ImpulseDirection.X, -ImpulseDirection.Y, 0, 0));
            //Add impulses to each player
            //p1.score -= 1000;
            //p2.score -= 1000;

            //handle collision resolution
            p1.position = new Vector3(p1.previousPosition.X, p1.previousPosition.Y, p1.previousPosition.Z);
            //p2.position = new Vector3(p2.previousPosition.X, p2.previousPosition.Y, p2.previousPosition.Z);

            p1.AddImpulses(newList);
            p2.AddImpulses(newList1);
        }


        //List sorted by points
        public List<Player> GetLeadersPoints()
        {            
            NPC = (from p in NPC orderby p.score descending select p).ToList();
            return NPC;
        }

        //List sorter by race pole position
        public List<Player> GetLeadersRank()
        {
            foreach (Player p in NPC)
            {
                float distance = Vector2.Distance(new Vector2(p.position.X, p.position.Y), Track.Waypoints[p.currentWaypoint]);
                p.progress = (p.currentLap * 10) + p.currentWaypoint - (distance / 100);
            }
            NPC = (from p in NPC orderby p.progress descending select p).ToList();
            return NPC;
        }

        public bool SomeoneFinished(int numberOfLaps)
        {
            List<Player> whoFinished = new List<Player>();
            foreach (Player p in NPC)
            {
                //  Winning condition
                if (p.currentLap >= numberOfLaps)
                {
                    if (p.currentWaypoint == 1)
                    {
                        if (p.position.Y > 88)
                        {
                            p.doneRace = true;
                            whoFinished.Add(p);
                        }
                    }
                }
            }

            if (whoFinished.Count > 1)
            {
                whoFinished = (from p in whoFinished orderby p.position.Y descending select p).ToList();
            }

            foreach (Player p in whoFinished)
            {                
                p.rank = ranking;
                ++ranking;
            }

            return (CountWhoFinished() > 0);
        }

        public int CountWhoFinished()
        {
            int counter = 0;
            foreach (Player p in NPC)
            {
                if (p.doneRace)
                {
                    ++counter;
                }
            }
            return counter;
        }
    }
}
