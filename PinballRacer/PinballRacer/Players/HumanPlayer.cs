using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PinballRacer.Players
{
    public class HumanPlayer : Player
    {
        KeyboardState lastKeyboardState;
        GamePadState padState;

        //  Specific keys for movement
        Keys forward;
        Keys reverse;
        Keys rightRoll;
        Keys leftRoll;

        public HumanPlayer()
        {            
            //  Default movement keys
            forward = Keys.Up;
            reverse = Keys.Down;
            rightRoll = Keys.Right;
            leftRoll = Keys.Left;
        }

        #region Initialization Methods
        public void InitializePosition(Vector3 aPosition, Vector3 aDirection, float aScale, Vector3 aRotation)
        {
            position = aPosition;
            direction = aDirection;
            scale = aScale;
            rotation = aRotation;
        }

        public void InitializeMovementKeys(Keys up, Keys down, Keys right, Keys left)
        {
            forward = up;
            reverse = down;
            rightRoll = right;
            leftRoll = left;
        }
        #endregion


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //  Setting necessary parameters            
            Vector3 previousVelocity = velocity;
            Vector3 previousRotation = rotation;            
            
            KeyboardState keyboardState = Keyboard.GetState();
            CheckPitchRollChanges(keyboardState);
            

            if (impulses.Count > 0)
            {
                velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            ApplyFriction(previousVelocity);
            UpdateRotation(previousRotation);
            position += velocity;            
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


        private void CheckPitchRollChanges(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(forward))
            {
                if (velocity.Y < MAX_SPEED)
                {
                    velocity.Y += 2 * SPEED_UP;
                }
            }

            if (keyboardState.IsKeyDown(reverse))
            {
                if (velocity.Y > -MAX_SPEED)
                {
                    velocity.Y += 2 * SLOW_DOWN;
                }
            }

            if (keyboardState.IsKeyDown(leftRoll))
            {
                if (velocity.X > -MAX_SPEED)
                {
                    velocity.X += 2 * SLOW_DOWN;
                }
            }

            if (keyboardState.IsKeyDown(rightRoll))
            {

                if (velocity.X < MAX_SPEED)
                {
                    velocity.X += 2 * SPEED_UP;
                }
            }
        }



        private void CheckPitchRollChanges(Keys key)
        {            
            if (key.Equals(forward))
            {                
                if (velocity.Y < MAX_SPEED)
                {
                    velocity.Y += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(reverse))
            {                
                if (velocity.Y > -MAX_SPEED)
                {
                    velocity.Y += 2 * SLOW_DOWN;
                }
                
            }
            else if (key.Equals(rightRoll))
            {
                if (velocity.X < MAX_SPEED)
                {
                    velocity.X += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(leftRoll))
            {
                if (velocity.X > -MAX_SPEED)
                {
                    velocity.X += 2 * SLOW_DOWN;
                }
            }
        }
        
        private bool CheckPreviousPressedKeys(Keys key)
        {
            bool keyPressed = false;

            if (lastKeyboardState.IsKeyUp(key))
            {
                keyPressed = true;
            }

            return keyPressed;
        }
    }
}
