using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track.Obstacles
{
    class Flipper : Obstacle
    {
        public enum Align { LEFT = 0, RIGHT};
        Matrix rotation;

        public Flipper(float x, float y, Model m, Matrix r)
        {
            rotation = r;
            model = m;
            position = new Vector3(x, y, 0f);
            scale = new Vector3(0.7f);
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
                    effect.World = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
        }
    }
}
