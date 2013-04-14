using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Players
{
    public abstract class Player
    {
        //  Constants (not sure if all are needed)
        protected const float COLLISION_TIME = 500.0f;
        public const float RADIUS = 0.5f;
        protected const float BOUNDARYX = 15f;
        protected const float BOUNDARYY = 19f;

        protected const float ANGULAR_VELOCITY = 0.1f;
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
        public Vector3 position { get; set; }
        protected Vector3 direction { get; set; }
        protected Vector3 velocity;
        protected float acceleration { get; set; }        

        //  Collision attributes
        protected bool hasCollided { get; set; }
        protected float timer { get; set; }        

        //  Model attributes
        public Model model { get; private set; }        
        public Vector3 color { get; set; }
        protected float scale { get; set; }
        protected Vector3 rotation;
        protected Vector3 previousRotation;
        protected float previousPitchChange { get; set; }
        protected float previousRollChange { get; set; }
        //protected float rotation { get; set; }  //  In degrees, converted to radians in draw method

        public bool hitSwitch;

        public void InitializeModel(Model aModel)
        {
            model = aModel;
        }

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
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            Matrix world = Matrix.CreateScale(scale) *
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.Z)) * 
                Matrix.CreateTranslation(position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = Color.Red.ToVector3();//new Vector3(0.2f, 0.2f, 0.2f);
                    //effect.DiffuseColor = color;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

        }

    }
}
