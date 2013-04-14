using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PinballRacer.Track;
using PinballRacer.Players;

namespace PinballRacer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TrackSpriteManager trackManager;
        PlayerSpriteManager playerManager;

        BasicEffect m_basicEffect;

        public static Matrix view = Matrix.CreateLookAt(new Vector3(20, 50, 70f), new Vector3(20, 50, 0), Vector3.UnitY);
        public static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 16 / 9, 1, 200);
        Model PinballTable;

        public enum states { box = 0, pause, play, main1, victory, instructions, last };
        states gameState = states.main1;
        SpriteFont font;
        private Texture2D smoke;
        bool pressed;
        Keys previousKey;

        //Camera attributes
        ChaseCamera camera;
        CameraView cameraView;
        float angleX = 0;
        float angleY = 0;
        float zoom = 0;

        public Game1()
        {
            //  Initialize drawable game components
            graphics = new GraphicsDeviceManager(this);
            trackManager = new TrackSpriteManager(this);
            playerManager = new PlayerSpriteManager(this);

            //  Add game components to the collection; xna will automatically call each update and draw method of every component.
            this.Components.Add(trackManager);
            this.Components.Add(playerManager);

            //  Setting up default root directory
            Content.RootDirectory = "Content";

            //  Defining the window size
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 600;
            pressed = false;
            //InitEffect();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            // Set up aspect ratio
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
                graphics.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            // TODO: use this.Content to load your game content here

            font = Content.Load<SpriteFont>("Score");
            smoke = Content.Load<Texture2D>("smoke");

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Initial camera load
            camera = new ChaseCamera();
            cameraView = CameraView.OVERVIEW;
            Player player = playerManager.GetHumanPlayer();
            UpdatePlayerCamera(player);
            camera.Reset();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            
            handleGameState(keyboardState);
            if (gameState == states.play)
            {
                cameraMotion(keyboardState);
                trackManager.track.Update(gameTime.ElapsedGameTime.Milliseconds);
            }

            // Update camera
            Player player = playerManager.GetHumanPlayer();
            UpdatePlayerCamera(player); // TODO: Add a player as a parameter

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == states.play || gameState == states.pause || gameState == states.instructions || gameState == states.victory)
            {
                #region ResetGraphic
                ResetGraphic();
                #endregion

                #region render 3D
                BeginRender3D();
                #endregion

                base.Draw(gameTime);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                  
            if (gameState == states.pause)
            {
                spriteBatch.Draw(smoke, new Vector2(0, 0), new Rectangle(0, 0, 2000, 2000), Color.FromNonPremultiplied(155, 155, 155, 155));            
                spriteBatch.DrawString(font, "PAUSED", new Vector2((graphics.PreferredBackBufferWidth / 2) - 25, graphics.PreferredBackBufferHeight / 2), Color.White);
                spriteBatch.DrawString(font, "(i)Instructions", new Vector2((graphics.PreferredBackBufferWidth / 2) - 65, graphics.PreferredBackBufferHeight / 2 + 30), Color.White);
            }
            if (gameState == states.main1)
            {
                spriteBatch.DrawString(font, "PINBALL RACERS", new Vector2(25, 25), Color.White);
                spriteBatch.DrawString(font, "Press Space to Begin",
                    new Vector2((graphics.PreferredBackBufferWidth / 2) - 105, graphics.PreferredBackBufferHeight / 2 + 30), Color.White);
            }
            if (gameState == states.instructions)
            {
                int startInstruction = 125;
                spriteBatch.Draw(smoke,
                    new Rectangle((int)(graphics.PreferredBackBufferWidth / 10), (int)(graphics.PreferredBackBufferHeight / 10),
                        (int)(graphics.PreferredBackBufferWidth - (graphics.PreferredBackBufferWidth / 5)),
                        (int)(graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 5))),
                    new Rectangle(0, 0, 1000, 1000),
                    Color.FromNonPremultiplied(155, 155, 155, 195)); 
                spriteBatch.DrawString(font, "INSTRUCTIONS", new Vector2((graphics.PreferredBackBufferWidth / 2) - 65, graphics.PreferredBackBufferHeight / 10 + 25), Color.White);
                spriteBatch.DrawString(font, "Use W/A/S/D to rotate camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 25), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Use Up/Down/Left/Right to move camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 45), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Use Q/E to move forward and backward", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 65), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "X to reset camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 85), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Space Next/Pause", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 125), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            } 
            spriteBatch.End();
        }

        public void InitEffect()
        {

            m_basicEffect = new BasicEffect(graphics.GraphicsDevice);
            m_basicEffect.View = view;
            m_basicEffect.Projection = projection;

            //Basic Effect Parameters
            m_basicEffect = new BasicEffect(graphics.GraphicsDevice);
            m_basicEffect.VertexColorEnabled = true;
            m_basicEffect.World = Matrix.CreateScale(1.0f);
            m_basicEffect.PreferPerPixelLighting = true;
        }


        /// <summary>
        /// This method is responsible for drawing the lines between the joints using the
        /// vertices method.
        /// </summary>
        private void DrawLines(Matrix view, Matrix projection, Vector3 start, Vector3 end)
        {
            VertexPositionColor[] primitiveList = new VertexPositionColor[2];
            primitiveList[0] = new VertexPositionColor(start, Color.Black);
            primitiveList[1] = new VertexPositionColor(end, Color.Black);
            m_basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draws Lines Between Vertices
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                primitiveList, 0,
                1);
        }

        public void cameraMotion(KeyboardState keyboardState)
        {
            //accepts all th input and reacts to it
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                trackManager.track.springLevel -= 0.01f;
            }
            if (keyboardState.IsKeyDown(Keys.D1))
            {
            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {  
            }
            if (keyboardState.IsKeyDown(Keys.D4))
            {
            }
            if (keyboardState.IsKeyDown(Keys.D5))
            {              
            }
            if (keyboardState.IsKeyDown(Keys.D8))
            {
                cameraView = CameraView.FIRST_PERSON;
            }
            if (keyboardState.IsKeyDown(Keys.D9))
            {
                cameraView = CameraView.THIRD_PERSON;
            }
            if (keyboardState.IsKeyDown(Keys.D0))
            {
                cameraView = CameraView.OVERVIEW;
            }
            if (keyboardState.IsKeyDown(Keys.X))
            {
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                angleX = 0.01f;
                view = view * Matrix.CreateRotationX(angleX);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                angleX = -0.01f;
                view = view * Matrix.CreateRotationX(angleX);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                angleY = 0.01f;
                view = view * Matrix.CreateRotationY(angleY);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                angleY = -0.01f;
                view = view * Matrix.CreateRotationY(angleY);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                zoom = 0.35f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                zoom = -0.35f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                zoom = 0.25f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                zoom = -0.25f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                zoom = -0.25f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
            if (keyboardState.IsKeyDown(Keys.J))
            {
                zoom = 0.25f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
        }

        public void ResetGraphic()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

        }
        public void BeginRender3D()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


        public void handleGameState(KeyboardState keyboardState)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
               keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Switch Full Screen
            if (keyboardState.IsKeyDown(Keys.F11))
            {
                graphics.ToggleFullScreen();
                 projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 1280 / 720, 1, 200);
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }
            if (gameState == states.main1)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.play;
                }
                else
                {
                    previousKey = Keys.D0;
                }
            }
            else if (gameState == states.victory)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.main1;
                }
            }
            else if (gameState == states.pause)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.play;
                }
                else if (keyboardState.IsKeyDown(Keys.I))
                {
                    gameState = states.instructions;
                }
            }
            else if (gameState == states.instructions)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.pause;
                }
            }
            else if (gameState == states.play)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.pause;
                }
                else if (keyboardState.IsKeyDown(Keys.I))
                {
                    gameState = states.instructions;
                }
            }
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                pressed = false;
            }
        }

        private void UpdatePlayerCamera(Player player)
        {
            camera.ChasePosition = player.position;
            //camera.ChaseDirection = player.direction;
            //camera.Up = player.Up;

            switch(cameraView)
            {
                case CameraView.FIRST_PERSON:
                    camera.DesiredPositionOffset = new Vector3(2, 2, 2);
                    //view = camera.View;
                    //projection = camera.Projection;
                    break;
                case CameraView.THIRD_PERSON:
                    camera.DesiredPositionOffset = new Vector3(0, 2000, 3500);
                    //view = camera.View;
                    //projection = camera.Projection;
                    break;
                default:
                    view = Matrix.CreateLookAt(new Vector3(20, 50, 70f), new Vector3(20, 50, 0), Vector3.UnitY);
                    projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 16 / 9, 1, 200);
                    break;
            }
        }
    }
}
