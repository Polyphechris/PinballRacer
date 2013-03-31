using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Obstacles
{
    class WallBumper : Wall
    {
        public WallBumper(float x, float y, Model m)
        {
            model = m;
            position = new Vector3(x, y, 2f);
            scale = new Vector3(1, 1, 0.5f);
        }

        public override Vector3 getResultingForce(Microsoft.Xna.Framework.Vector3 player)
        {
            return Vector3.Zero;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, 0f, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
        }
    }
}
