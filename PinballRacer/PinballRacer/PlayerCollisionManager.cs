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

        public List<Player> NPC;
        RaceTrack Track;
        ContentManager content;
        Model wall;
        Model ball;

        public PlayerCollisionManager(ContentManager c, RaceTrack t)
        {
            Track = t;
            content = c;
            NPC = new List<Player>();
        }

        public void InitializePlayers(List<NpcPlayer> players, Player human)
        {
            foreach (NpcPlayer npc in players)
            {
                npc.model = human.model;
                NPC.Add(npc);
            }
            NPC.Add(human);
        }

        public void update(GameTime time)
        {
            foreach (Player p in NPC)
            {
                //Check P2P collision
                foreach (Player p2 in NPC)
                {
                    if (p2 != p)
                    {
                        if (Collision(p, p2))
                        {
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
                        Impulses.Add(new Vector4(i.X, i.Y, i.Z, 0));
                    }
                }
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
                return true;
            }
            return false;
        }

        public void HandleCollision(Player p1, Player p2)
        {
            //Recompute a path, apply bounce forces, steer
            Vector3 ImpulseDirection = p1.position - p2.position;
            ImpulseDirection.Normalize();

            //Add impulses to each player
            // p1.impulses.Add(ImpulseDirection);
           // p2.impulses.Add(-ImpulseDirection);
        }
    }
}
