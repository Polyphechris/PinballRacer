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
            modelRotation = Quaternion.Identity;
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
            //  Setting necessary parameters
            bool yawChanged = false;
            bool pitchChanged = false;
            
            previousRotation = rotation;            
            
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();
            
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            foreach (Keys key in keys)
            {             
                CheckPitchRollChanges(key,  ref yawChanged, ref pitchChanged);
            }

            ApplyFriction(yawChanged, pitchChanged);
            UpdateRotation();

            position += velocity;
        }

        private void ApplyFriction(bool hasYawChanged, bool hasPitchChanged)
        {
            if (hasYawChanged)
            {
                if (velocity.X > SPEED_UP)
                {
                    velocity.X += 0.5f * SLOW_DOWN;
                }
                else if (velocity.X < SLOW_DOWN)
                {
                    velocity.X += 0.5f * SPEED_UP;
                }
                else
                {
                    velocity.X = 0.0f;
                }
            }
            else
            {
                if (velocity.X > SPEED_UP)
                {
                    velocity.X += 1.5f * SLOW_DOWN;
                }
                else if (velocity.X < SLOW_DOWN)
                {
                    velocity.X += 1.5f * SPEED_UP;
                }
                else
                {
                    velocity.X = 0.0f;
                }
            }

            if (hasPitchChanged)
            {
                if (velocity.Y > SPEED_UP)
                {
                    velocity.Y += 0.5f * SLOW_DOWN;
                }
                else if (velocity.Y < SLOW_DOWN)
                {
                    velocity.Y += 0.5f * SPEED_UP;
                }
                else
                {
                    velocity.Y = 0.0f;
                }
            }
            else
            {
                if (velocity.Y > SPEED_UP)
                {
                    velocity.Y += 1.5f * SLOW_DOWN;
                }
                else if (velocity.Y < SLOW_DOWN)
                {
                    velocity.Y += 1.5f * SPEED_UP;
                }
                else
                {
                    velocity.Y = 0.0f;
                }

            }
        }
        private void UpdateRotation()
        {
            if (!velocity.Equals(Vector3.Zero))
            {
                //  velocity = angular velocity * rotation             
                
                rotation.X = velocity.X / ANGULAR_VELOCITY + previousRotation.X;
                rotation.X = RebalanceRotation(rotation.X);

                //  Condition to fix the yaw/pitch problem
                if (rotation.X > 90.0f && rotation.X < 270.0f)
                {
                    rotation.Y = velocity.Y / ANGULAR_VELOCITY + previousRotation.Y;
                    rotation.Y = RebalanceRotation(rotation.Y);
                }
                else
                {
                    rotation.Y = -velocity.Y / ANGULAR_VELOCITY + previousRotation.Y;
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

        private void CheckPitchRollChanges(Keys key, ref bool hasYawChanged, ref bool hasPitchChanged)
        {
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            if (key.Equals(forward))
            {
                hasPitchChanged = true;
                if (velocity.Y != MAX_SPEED)
                {
                    velocity.Y += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(reverse))
            {
                hasPitchChanged = true;
                if (velocity.Y != -MAX_SPEED)
                {
                    velocity.Y += 2 * SLOW_DOWN;
                }
                
            }
            else if (key.Equals(rightRoll))
            {
                hasYawChanged = true;
                if (velocity.X != MAX_SPEED)
                {
                    velocity.X += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(leftRoll))
            {
                hasYawChanged = true;
                if (velocity.X != -MAX_SPEED)
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
