using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Players;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Track.Walls;

namespace PinballRacer.Track.Obstacles
{
    class Switch : Obstacle
    {
        const float LANE_WIDTH = 1;
        const float LANE_HEIGHT = 4;
        const float LIGHT_RADIUS = 0.75f;
        const int LIGHT_VALUE = 1000;
        const int JACKPOT_VALUE = 1000000;
        const float IMMUNE_TIME = 1000f;

        //X, Y , Z, ON(0)/OFF(1)
        Vector4[] lanes;
        float[] immuneTimers;

        Model Light;
        Model Wall;
        Vector2 bottomLeft;
        List<Wall> walls;
        int lightsOn;

        bool jackpot;

        public Switch(Vector2 tl, Model l, Model w)
        {
            jackpot = false;
            lightsOn = 0;
            Light = l;
            Wall = w;
            bottomLeft = tl;
            immuneTimers = new float[3] { 0, 0, 0 };
            lanes = new Vector4[3] { Vector4.Zero, Vector4.Zero, Vector4.Zero };
            for (int i = 0; i < 3; ++i)
            {
                lanes[i] = new Vector4(bottomLeft.X + (i * 3) + 1.5f, bottomLeft.Y + LANE_HEIGHT / 2, -0.5f, 0);
            }
            InitializeWalls();
            CollisionBox = new Rectangle((int)bottomLeft.X - 1, (int)bottomLeft.Y - 1, (int)LANE_WIDTH * 10 + 2, (int)LANE_HEIGHT + 2);
        }

        private void InitializeWalls()
        {
            walls = new List<Wall>();
            for (int i = 0; i <= 3; ++i)
            {
                for (int j = 0; j <= LANE_HEIGHT; ++j)
                {
                    walls.Add(new WallRegular(bottomLeft.X + (i * 3), bottomLeft.Y + j, Wall));
                }
            }
        }

        public bool CheckLight(Player player)
        {
            //Check what lane we are in if any
            int lightIndex = (int)((player.position.X - bottomLeft.X) / 3);
            if (lightIndex >= 0 && lightIndex < 3)
            {
                float distance = Vector3.Distance(player.position, new Vector3(lanes[lightIndex].X, lanes[lightIndex].Y, lanes[lightIndex].Z));
                if (distance <= Player.RADIUS + LIGHT_RADIUS && immuneTimers[lightIndex] == 0)
                {
                    //Set the player to having passed over so it doesnt switch twice in <1s
                    immuneTimers[lightIndex] = 1;

                    //Switch the light's status
                    int score = LIGHT_VALUE;
                    if (lanes[lightIndex].W == 0)
                    {
                        lanes[lightIndex] += new Vector4(0, 0, 0, 1);
                        lightsOn++;
                        if (lightsOn == 3) { score = JACKPOT_VALUE; lightsOn = 0; lanes[0].W = 0; lanes[1].W = 0; lanes[2].W = 0; }
                    }
                    else
                    {
                        lightsOn--;
                        lanes[lightIndex] -= new Vector4(0, 0, 0, 1);
                    }
                    //Give player a score
                    //TO DO
                }
            }
            return false;
        }
        
        public override Vector3 getResultingForce(Player p)
        {
            Vector3 player = p.position;
            CheckLight(p);
            Vector3 wallHit = Vector3.Zero;
            foreach (Wall w in walls)
            {
                wallHit += w.getResultingForce(p);
            }
          //  throw new NotImplementedException();
            return wallHit;
        }

        public override void update(float time)
        {
            base.update(time);

            //Update Immunity timers so we cant pass over and make it flicker
            for (int i = 0; i < 3; ++i)
            {
                if (immuneTimers[i] > 0)
                {
                    immuneTimers[i] += time;
                    if (immuneTimers[i] >= IMMUNE_TIME)
                    {
                        immuneTimers[i] = 0;
                    }
                }
            }
            foreach (Wall w in walls)
            {
                w.update(time);
            }
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (Obstacle wall in walls)
            {
                wall.draw(view, projection);
            }
            for (int i = 0; i < 3; ++i)
            {                
                foreach (ModelMesh mesh in Light.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.EmissiveColor = new Vector3(0,0,0);
                       // effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                       // effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                        effect.World = Matrix.CreateScale(new Vector3(0.75f, 0.75f, 0.1f)) * Matrix.CreateTranslation(new Vector3(lanes[i].X, lanes[i].Y, lanes[i].Z));
                        effect.View = view;
                        effect.Projection = projection;
                        effect.Alpha = 0.7f;
                        if (lanes[i].W == 1)
                        {
                            effect.EmissiveColor = new Vector3(1, 1, 0.1f);
                            effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.1f);
                        }
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
