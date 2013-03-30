using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Player
{
    public abstract class Player
    {
        //  Constants (not sure if all are needed)
        protected const float COLLISION_TIME = 500.0f;
        protected const float RADIUS = 0.5f;
        protected const float BOUNDARYX = 15f;
        protected const float BOUNDARYY = 19f;

        protected const float SPEED_UP = 0.005f;
        protected const float SLOW_DOWN = -0.005f;
        protected const float MAX_ACC = 0.005f;
        protected const float MIN_ACC = -0.005f;
        protected const float MAX_SPEED = 4f;
        protected const float FRICTION = 1f;
        protected const float CHASE_SPEED_UP = 1.2f;
        protected const float IMMUNITY = 3000f;
        protected const float CONE = (float)Math.PI / 4;

        //  Movement attributes
        protected Vector3 position { get; set; }
        protected Vector3 direction { get; set; }
        protected float velocity { get; set; }
        protected float acceleration { get; set; }

        //  Collision attributes
        protected bool hasCollided { get; set; }
        protected float timer { get; set; }        

        //  Model attributes
        private Model model { get; set; }        
        private Vector3 color { get; set; }
        private float scale { get; set; }
        private float rotation { get; set; }
        

        public void Direction()
        {
            //  Calculates the direction that the player needs to steer at. It will adjust the rotation and

        }


        public bool CheckCollision()
        {
            //TODO set logic for collision, needs arguments in the method
            return false;
        }

        public void CollisionAnimation(float elapsedTime)
        {
            if (hasCollided)
            {
                timer += elapsedTime;
                if (timer > COLLISION_TIME)
                {
                    hasCollided = false;
                    timer = 0.0f;
                }
                //TODO animation
            }
        }

        public abstract void Update(GameTime gameTime);
        
        public void Draw(Matrix view, Matrix projection)
        {
            Matrix world = Matrix.CreateScale(scale) *
                 Matrix.CreateRotationY(rotation) *
                 Matrix.CreateTranslation(position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

                    effect.DiffuseColor = color;

                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

        }

    }
}
