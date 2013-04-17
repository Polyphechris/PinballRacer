using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Obstacles
{
    public class Bumper : Obstacle
    {
        const int SCORE = 5000;
        public const float RADIUS = 1.15f;
        public const float E = 20f;

        public Bumper(float x, float y, Model m)
        {
            score = SCORE;
            model = m;
            position = new Vector3(x, y, 0.5f);
            scale = new Vector3(0.7f, 0.7f, 0.025f);
            isHit = false;            
            CollisionBox = new Rectangle((int)(x - (RADIUS/2)),(int)( y - (RADIUS/2)), (int)RADIUS * 2, (int)RADIUS * 2);
        }

        public override Vector3 getResultingForce(Player p)
        {
            Vector3 player = p.position;
            if (Vector3.Distance(player, position) <= RADIUS + Player.RADIUS)
            {
                isHit = true;
                Vector3 force = player - position;
                force.Normalize();
                p.score += score;
                return new Vector3(force.X, force.Y, 0) * E;
            }
            return Vector3.Zero;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, 0, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                    if (isHit)
                    {
                        effect.DirectionalLight1.SpecularColor = new Vector3(1f, 1f, 1f);
                        effect.EmissiveColor = new Vector3(0.2f, 0.2f, 0.2f);
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    }
                    else
                    {
                        effect.DirectionalLight1.SpecularColor = Vector3.Zero;
                        effect.EmissiveColor = Vector3.Zero;
                    }
                }
                mesh.Draw();
            }
        }
    }
}
