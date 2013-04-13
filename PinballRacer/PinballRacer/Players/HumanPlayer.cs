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
        Keys accelerate;
        Keys brake;
        Keys turnRight;
        Keys turnLeft;

        public HumanPlayer()
        {
            //  Default movement keys
            accelerate = Keys.Up;
            brake = Keys.Down;
            turnRight = Keys.Right;
            turnLeft = Keys.Left;
        }

        #region Initialization Methods
        public void InitializePosition(Vector3 aPosition, Vector3 aDirection, float aScale, float aRotation)
        {
            position = aPosition;
            direction = aDirection;
            scale = aScale;
            rotation = aRotation;
        }

        public void InitializeMovementKeys(Keys up, Keys down, Keys right, Keys left)
        {
            accelerate = up;
            brake = down;
            turnRight = right;
            turnLeft = left;
        }
        #endregion


        public override void Update(GameTime gameTime)
        {
            //  Resetting necessary parameters
            bool isAccelerating = false; //only considered accelerating whenever accelerate button is used.
            bool isSlowingDown = false;
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();

            
            foreach (Keys key in keys)
            {
                if (key.Equals(accelerate))
                {
                    isAccelerating = true;                    
                }
                else if (key.Equals(brake))
                {                    
                    isSlowingDown = true;
                }
                else if (key.Equals(turnRight))
                {
                    rotation += 5.0f;
                }
                else if (key.Equals(turnLeft))
                {
                    rotation -= 5.0f;
                }
            }

            CheckSpeed(isAccelerating, isSlowingDown);

            //  Calculating direction

            float horizontalDirection = (float)(Math.Cos(MathHelper.ToRadians(rotation)));
            float verticalDirection = (float)(Math.Sin(MathHelper.ToRadians(rotation) - Math.PI));
            direction = new Vector3(horizontalDirection, verticalDirection, 0.0f);

            position += velocity * direction;
        }

        
        private void CheckSpeed(bool accelerating, bool slowingDown)
        {            
            if(accelerating)   //  Giving priority to acceleration if both speedUp and brake are pressed at the same time
            {
                velocity += SPEED_UP;            
            }
            else
            {
                if (velocity > 0.0f)
                {
                    if (slowingDown)
                    {
                        velocity += SLOW_DOWN * 2;  //  Make brake slow down by twice the amount
                    }
                    else if (!accelerating && !slowingDown)
                    {
                        velocity += SLOW_DOWN;
                    }
                }                
                else
                {
                    velocity = 0.0f;
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
