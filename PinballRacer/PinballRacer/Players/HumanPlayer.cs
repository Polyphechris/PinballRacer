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

        //  Specific keys for movement
        Keys speedUp;
        Keys slowDown;
        Keys turnRight;
        Keys turnLeft;

        public HumanPlayer()
        {
            //  Default movement keys
            speedUp = Keys.Up;
            slowDown = Keys.Down;
            turnRight = Keys.Right;
            turnLeft = Keys.Left;
        }

        public void InitializeMovementKeys(Keys up, Keys down, Keys right, Keys left)
        {
            speedUp = up;
            slowDown = down;
            turnRight = right;
            turnLeft = left;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] keys = keyboardState.GetPressedKeys();

            foreach (Keys key in keys)
            {
                if (key.Equals(speedUp))
                {
                    velocity += SPEED_UP;
                }
                if (key.Equals(slowDown))
                {
                    velocity += SLOW_DOWN;
                }
                if (key.Equals(turnRight))
                {
                    
                }
                if (key.Equals(turnLeft))
                {

                }
            }            
        }
        
        private bool CheckPressedKeys(Keys key)
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
