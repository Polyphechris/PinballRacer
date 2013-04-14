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
            //CheckSpeed(isRollingForward, isRollingSideways);

            //  Calculating direction

            //float horizontalDirection = (float)(Math.Cos(MathHelper.ToRadians(rotation.X)));
            //float verticalDirection = (float)(Math.Sin(MathHelper.ToRadians(rotation.Y) - Math.PI));
            float horizontalDirection = 5.0f;
            float verticalDirection = 0.0f;

            direction = new Vector3(horizontalDirection, verticalDirection, 0.0f);

            position += velocity;//velocity * direction;            
        }

        private void CheckMomentum(bool hasPitchChanged, bool hasRollChanged)
        {
            if (!velocity.Equals(Vector3.Zero))
            {
                //  velocity = angular velocity * rotation
                if (!hasPitchChanged)
                {
                    if (velocity.Y > 0.01f)
                    {
                        velocity.Y += SLOW_DOWN;
                    }
                    else if (velocity.Y < -0.01f)
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
                    if (velocity.X > 0.01f)
                    {
                        velocity.X += SLOW_DOWN;
                    }
                    else if (velocity.X < -0.01f)
                    {
                        velocity.X += SPEED_UP;
                    }
                    else
                    {
                        velocity.X = 0.0f;
                    }
                }
                
                rotation.X = velocity.X / ANGULAR_VELOCITY + RebalanceRotation(previousRotation.X);
                rotation.Y = -velocity.Y / ANGULAR_VELOCITY + RebalanceRotation(previousRotation.Y);                
            }
        }

        /// <summary>
        /// Checks to see if rotations exceed -360/360 degrees
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
