using System;
using Microsoft.Xna.Framework;
using PinballRacer.Players.Strategies;

namespace PinballRacer.Players
{
    public class NpcPlayer : Player
    {
        public Strategy pickStrategy;

        #region Initialization Methods
        public void InitializePosition(Vector3 aPosition, Vector3 aDirection, float aScale, Vector3 aRotation)
        {
            position = aPosition;
            direction = aDirection;
            scale = aScale;
            rotation = aRotation;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Game1.launched)
            {
                //  Setting necessary parameters            
                Vector3 previousVelocity = velocity;
                Vector3 previousRotation = rotation;
                bool up = false;
                bool down = false;
                bool left = false;
                bool right = false;

                // TODO: Get the direction of where to head next (using the strategies)

                // Set the direction booleans as needed
                /*
                if (direction.X > 0) { right = true; }
                if (direction.X < 0) { left = true; }
                if (direction.Y > 0) { up = true; }
                if (direction.Y < 0) { down = true; }
                */

                //  yaw(spin), pitch (forward/backward), roll (sideways)
                if (Game1.closeLoader)
                {
                    CheckPitchRollChanges(up, down, left, right);
                    SetSteering();
                    //if (impulses.Count > 0)
                    //{
                    velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //}
                    //velocity += acceleration;
                    
                    ApplyFriction(previousVelocity);
                    UpdateRotation(previousRotation);
                    previousPosition = new Vector3(position.X, position.Y, position.Z);
                    velocity = truncate(velocity, SPEED_UP * 38);
                    position += velocity;
                    //position += velocity;
                }
                else
                {
                    velocity = new Vector3(0, 1.4f, 0) - new Vector3(0, gameTime.ElapsedGameTime.Milliseconds / 100, 0);
                    previousPosition = new Vector3(position.X, position.Y, position.Z);
                    position += velocity;
                }                
            }
        }

        private void ApplyFriction(Vector3 aPreviousVelocity)
        {
            if (aPreviousVelocity.Length() < 0.025f)
            {
                velocity *= STATIC_FRICTION;
            }
            else
            {
                if (velocity.Length() > 0.005f)
                {
                    velocity *= KINETIC_FRICTION;
                }
                else
                {
                    velocity = Vector3.Zero;
                }
            }
        }

        /// <summary>
        /// Formula: velocity = angular velocity * rotation
        /// </summary>
        private void UpdateRotation(Vector3 aPreviousRotation)
        {
            if (!velocity.Equals(Vector3.Zero))
            {
                rotation.X = velocity.X / ANGULAR_VELOCITY + aPreviousRotation.X;
                rotation.X = RebalanceRotation(rotation.X);

                //  Condition to fix the yaw/pitch problem
                if (rotation.X > 90.0f && rotation.X < 270.0f)
                {
                    rotation.Y = velocity.Y / ANGULAR_VELOCITY + aPreviousRotation.Y;
                    rotation.Y = RebalanceRotation(rotation.Y);
                }
                else
                {
                    rotation.Y = -velocity.Y / ANGULAR_VELOCITY + aPreviousRotation.Y;
                    rotation.Y = RebalanceRotation(rotation.Y);
                }
            }
        }

        /// <summary>
        /// Checks to see if rotations exceed 0-360 degrees
        /// </summary>
        private float RebalanceRotation(float aRotation)
        {
            float rebalancedRotation = aRotation;

            if (aRotation > 360.0f)
            {
                rebalancedRotation -= 360.0f;
            }
            else if (aRotation < 0.0f)
            {
                rebalancedRotation += 360.0f;
            }
            return rebalancedRotation;
        }

        private void CheckPitchRollChanges(bool up, bool down, bool left, bool right)
        {
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            if (up)
            {
                if (velocity.Y < MAX_SPEED)
                {
                    velocity.Y += 2 * SPEED_UP;
                }
            }
            
            if (down)
            {
                if (velocity.Y > -MAX_SPEED)
                {
                    velocity.Y += 2 * SLOW_DOWN;
                }
            }
            
            if (right)
            {
                if (velocity.X < MAX_SPEED)
                {
                    velocity.X += 2 * SPEED_UP;
                }
            }
            
            if (left)
            {
                if (velocity.X > -MAX_SPEED)
                {
                    velocity.X += 2 * SLOW_DOWN;
                }
            }
        }

        private void SetSteering()
        {
            Vector3 desiredVelocity = Vector3.Zero;
            if (path != null)
            {
                //Gets the desired direction
                desiredVelocity = MAX_SPEED * path.getDirection(0, position);
                if (path.checkEnd())
                {
                    path = null;
                }
            
                Vector3 steering = desiredVelocity - velocity;
                steering.Z = 0;
                steering = Vector3.Normalize(steering) * MAX_ACC;            
                acceleration += steering;
                // acceleration += truncate(acceleration + steering, MAX_ACC);
            }
        }

        private Vector3 truncate(Vector3 velocity, float max)
        {
            velocity.Z = 0;
            velocity = Vector3.Normalize(velocity) * max;
            return velocity;
        }
    }
}
