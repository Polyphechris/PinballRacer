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
            bool pitchChanged = false;
            bool rollChanged = false;
            previousRotation = rotation;            
            
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();
            
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            foreach (Keys key in keys)
            {
                if (key.Equals(forward))     //  rolling forward
                {
                    pitchChanged = true;
                    CheckPitchRollChanges(key);
                }
                else if (key.Equals(reverse))     //  rolling backward
                {
                    pitchChanged = true;
                    CheckPitchRollChanges(key);
                }
                else if (key.Equals(rightRoll)) //  rolling right
                {
                    rollChanged = true;
                    CheckPitchRollChanges(key);
                    //rotation.X += 5.0f;
                }
                else if (key.Equals(leftRoll))  //  rolling left
                {
                    rollChanged = true;
                    CheckPitchRollChanges(key);
                    //rotation -= 5.0f;
                }
            }
            CheckMomentum(pitchChanged, rollChanged);

            position += velocity;//velocity * direction;            
        }

        private void CheckMomentum(bool hasPitchChanged, bool hasRollChanged)
        {
            if (!velocity.Equals(Vector3.Zero))
            {
                //  velocity = angular velocity * rotation
                if (!hasPitchChanged)
                {
                    if (velocity.Y > SPEED_UP)
                    {
                        velocity.Y += SLOW_DOWN;
                    }
                    else if (velocity.Y < SLOW_DOWN)
                    {
                        velocity.Y += SPEED_UP;
                    }
                    else
                    {
                        velocity.Y = 0.0f;
                    }
                }

                if (!hasRollChanged)
                {
                    if (velocity.X > SPEED_UP)
                    {
                        velocity.X += SLOW_DOWN;
                    }
                    else if (velocity.X < SLOW_DOWN)
                    {
                        velocity.X += SPEED_UP;
                    }
                    else
                    {
                        velocity.X = 0.0f;
                    }
                }
                
                rotation.X = velocity.X / ANGULAR_VELOCITY + previousRotation.X;
                rotation.X = RebalanceRotation(rotation.X);
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

        private void CheckPitchRollChanges(Keys key)
        {
            //  yaw(spin), pitch (forward/backward), roll (sideways)
            if (key.Equals(forward))
            {
                if (velocity.Y != MAX_SPEED)
                {
                    velocity.Y += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(reverse))
            {
                if (velocity.Y != -MAX_SPEED)
                {
                    velocity.Y += 2 * SLOW_DOWN;
                }
                
            }
            else if (key.Equals(rightRoll))
            {
                if (velocity.X != MAX_SPEED)
                {
                    velocity.X += 2 * SPEED_UP;
                }
            }
            else if (key.Equals(leftRoll))
            {
                if (velocity.X != -MAX_SPEED)
                {
                    velocity.X += 2 * SLOW_DOWN;
                }
            }
            //if (key.Equals(forward))
            //{
            //    previousPitchChange = previousPitchChange + 1.0f;
            //    rotation.Y += previousPitchChange;
            //}
            //else if (key.Equals(reverse))
            //{
            //    previousPitchChange = previousPitchChange - 1.0f;
            //    rotation.Y += previousPitchChange;
            //}
            //else if (key.Equals(leftRoll))
            //{
            //    previousRollChange = previousRollChange + 1.0f;
            //    rotation.Z += previousRollChange;
            //}
            //else if (key.Equals(rightRoll))
            //{
            //    previousRollChange = previousRollChange - 1.0f;
            //    rotation.Z += previousRollChange;
            //}
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
