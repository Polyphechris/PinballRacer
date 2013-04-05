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
        public const float E = 1f;
        public const float INTERVAL = 1000f;
        public float W1 = 3f;
        public float W2= 0.75f;
        public float START_D = -0.3f;
        public float MAX_D = (float)Math.PI/4;

        public enum Align { LEFT = 0, RIGHT};
        Matrix rotation;
        private bool inverted;

        float idle_timer;
        float angle;
        float w; //angular speed
        float delta_angle;
        public enum states { FIRING, IDLE, RELOAD }; //Shooting the ball, doing nothing, going back down
        states state;

        public Flipper(float x, float y, Model m, float a, bool i)
        {
            w = 0;
            idle_timer = 0f;
            state = states.IDLE;
            inverted = i;
            angle = a;
            model = m;
            position = new Vector3(x, y, 0f);
            scale = new Vector3(0.7f);
        }

        public override Vector3 getResultingForce(Microsoft.Xna.Framework.Vector3 player)
        {
            return Vector3.Zero;
        }

        public override void update(float time)
        {
            if (state == states.IDLE)
            {
                w = 0;
                idle_timer += time;
                if (idle_timer >= INTERVAL)
                {
                    idle_timer = 0;
                    state = states.FIRING;
                }
            }
            else if (state == states.FIRING)
            {
                w = W1;
                int minus = 1;
                if (inverted) { minus = -1; }
                angle += w * minus * time / 1000;
                delta_angle += w * time / 1000;
                //if we reach the max start going back down
                if (delta_angle >= MAX_D) 
                    state = states.RELOAD;                
            }
            else if(state == states.RELOAD)
            {
                w = W2;
                int minus = -1;
                if (inverted) { minus = 1; }
                angle += w * minus * time / 1000;
                delta_angle -= w * time / 1000;
                //if we reach the max start going back down
                if (delta_angle <= START_D) 
                    state = states.IDLE;
            }
        }

        public override void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {                    
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = new Vector3(0f, -1f, -1f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);// Shinnyness/reflexive
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    //effect.Alpha = 0.8f;
                }
                mesh.Draw();
            }
        }
    }
}
