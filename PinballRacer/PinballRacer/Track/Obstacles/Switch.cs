using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PinballRacer.Player;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Track.Walls;

namespace PinballRacer.Track.Obstacles
{
    class Switch : Obstacle
    {
        const float LANE_WIDTH = 1;
        const float LANE_HEIGHT = 4;

        bool[] lanes;

        Model Light;
        Model Wall;
        Vector2 bottomLeft;
        List<Wall> walls;

        public Switch(Vector2 tl, Model l, Model w)
        {
            Light = l;
            Wall = w;
            bottomLeft = tl;
            lanes = new bool[3] { false, false, false};
            InitializeWalls();
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

        public bool CheckLight()
        {
            //Check if player is between bottomLeft and bottomLeft + (8,LANE_HEIGHT)

            //Switch the light's status
            //Set the player to having passed over so it doesnt switch twice in <1s
            return false;
        }

        
        public override Vector3 getResultingForce(Vector3 player)
        {
            throw new NotImplementedException();
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
                        effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                        effect.World = Matrix.CreateScale(0.75f) * Matrix.CreateTranslation(new Vector3(bottomLeft.X + (i*3) + 1.5f, bottomLeft.Y + LANE_HEIGHT/2, -1f));
                        effect.View = view;
                        effect.Projection = projection;
                        //effect.Alpha = 0.8f;
                        if (lanes[i])
                            effect.EmissiveColor = new Vector3(1,1,1);
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
