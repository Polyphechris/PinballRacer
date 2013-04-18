using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Players;

namespace PinballRacer.Track.Walls
{
    class Floor : Wall
    {
        public Vector3 colour { get; private set; }

        public Floor(float x, float y, Model m, Vector3 c)
        {
            isHit = false;
            model = m;
            position = new Vector3(x,  y, -0.5f);
            scale = new Vector3(0.5f);
            colour = c;
        }

        public override Vector3 getResultingForce(Player p)
        {
            //throw new NotImplementedException();
            return Vector3.Zero;
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {                    
                    //Comment this out for smoothing the walls and floors
                    effect.EnableDefaultLighting();                    
                    effect.AmbientLightColor = colour;                    
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
