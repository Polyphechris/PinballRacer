using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track
{
    public abstract class Obstacle
    {
        protected Model model;
        protected int score;
        protected Vector3 position;
        protected Vector3 scale;

        //How much force is transmited back to the ball
        protected float elasticity;

        public void update(float time) { }

        public virtual void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Enabled = true; 
                    effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);
                    effect.AmbientLightColor = new Vector3(0.55f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
        }

        //Takes a player's position and returns a resulting force based on shape and elasticity
        public abstract Vector3 getResultingForce(Vector3 player);

        private bool IsCollision(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            for (int meshIndex1 = 0; meshIndex1 < model1.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = model1.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(world1);

                for (int meshIndex2 = 0; meshIndex2 < model2.Meshes.Count; meshIndex2++)
                {
                    BoundingSphere sphere2 = model2.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(world2);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            return false;
        }
    }
}
