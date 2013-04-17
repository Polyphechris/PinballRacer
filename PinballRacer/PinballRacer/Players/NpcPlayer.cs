using System;
using Microsoft.Xna.Framework;
using PinballRacer.Players.Strategies;

namespace PinballRacer.Players
{
    public class NpcPlayer : Player
    {
        Strategy pickStrategy;

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
            CheckPitchRollChanges(up, down, left, right);

            if (impulses.Count > 0)
            {
                velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            ApplyFriction(previousVelocity);
            UpdateRotation(previousRotation);

            position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000;
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
    }
}
