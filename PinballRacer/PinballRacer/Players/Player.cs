using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PinballRacer.Track.Pathfinding;

namespace PinballRacer.Players
{
    public abstract class Player
    {
        //  Constants (not sure if all are needed)
        protected const float COLLISION_TIME = 500.0f;
        public const float RADIUS = 0.5f;
        protected const float BOUNDARYX = 15f;
        protected const float BOUNDARYY = 19f;

        protected const float ANGULAR_VELOCITY = 0.02f;     // Increasing this reduces the rotation speed
        protected const float SPEED_UP = 0.005f;
        protected const float SLOW_DOWN = -0.005f;
        protected const float MAX_ACC = 1f;
        protected const float MIN_ACC = -0.005f;
        protected const float MAX_SPEED = 4f;
        protected const float KINETIC_FRICTION = 0.95f;
        protected const float STATIC_FRICTION = 0.75f;
        protected const float CHASE_SPEED_UP = 1.2f;
        protected const float IMMUNITY = 3000f;
        protected const float CONE = (float)Math.PI / 4;

        //  Movement attributes
        public Vector3 position { get; set; }
        public Vector3 previousPosition { get; set; }
        protected Vector3 direction { get; set; }
        public Vector3 velocity;
        protected Vector3 acceleration { get; set; }        

        //  Collision attributes
        protected bool hasCollided { get; set; }
        protected float timer { get; set; }        

        //  Model attributes
        public Model model { get; private set; }        
        public Vector3 color { get; set; }
        public float scale { get; protected set; }
        protected Vector3 rotation;        

        protected List<Vector4> impulses = new List<Vector4>();
        protected Path path;
        public int score;
        public int rank;
        public int currentWaypoint;
        public int currentLap;
        public float progress;
        public string name;

        public void InitializeModel(Model aModel)
        {
            currentWaypoint = 0;
            currentLap = 0;
            model = aModel;
        }

        public int ImpulseCount()
        {
            return impulses.Count;
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

        public virtual void Update(GameTime gameTime)
        {
            acceleration = Vector3.Zero;
            for (int i = 0; i < impulses.Count; ++i)
            {
                impulses[i] += new Vector4(0, 0, 0, gameTime.ElapsedGameTime.Milliseconds);
                if (impulses[i].W > 50)
                {
                    impulses.RemoveAt(i);
                }
                else
                {
                    Vector3 J = new Vector3(impulses[i].X, impulses[i].Y, impulses[i].Z);
                    acceleration += J;
                }
            }
        }
        
        public void AddImpulses(List<Vector4> newImpulses)
        {
            impulses.AddRange(newImpulses);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            //if (path != null)
            //    path.Draw(view, projection, model);
            //  yaw(spin), pitch (forward/backward), roll (sideways)            
            Matrix world = Matrix.CreateScale(scale) *
                    Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotation.X), MathHelper.ToRadians(rotation.Y), MathHelper.ToRadians(rotation.Z)) *
                    Matrix.CreateTranslation(position);            

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = color;// Color.Red.ToVector3();//new Vector3(0.2f, 0.2f, 0.2f);
                    //effect.DiffuseColor = color;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public bool NullPath()
        { return path == null; }

        public void SetPath(Path p)
        {
            path = p;
        }
                
        public Vector2 TrackToMap()
        {
            return Vector2.Zero;
        }

        public void PlayerFinishedRace(int ranking)
        {
            rank = ranking;            
        }
    }
}
