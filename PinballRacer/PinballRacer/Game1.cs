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
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TrackSpriteManager trackManager;
        PlayerSpriteManager playerManager;
        PlayerCollisionManager collisionManager;

        public static BasicEffect m_basicEffect;

        public static Matrix view = Matrix.CreateLookAt(new Vector3(20, 50, 70f), new Vector3(20, 50, 0), Vector3.UnitY);
        public static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 16 / 9, 1, 200);
        Model PinballTable;

        public enum states { box = 0, pause, play, main1, victory, instructions, last };
        states gameState = states.main1;
        SpriteFont font;
        private Texture2D smoke;
        bool pressed;
        bool pressed2;
        bool showBoard;
        Keys previousKey;

        //Camera attributes
        ChaseCamera camera;
        CameraView cameraView;
        float camAngle = 0;
        float angleX = 0;
        float angleY = 0;
        float zoom = 0;

        // Spring timer
        float timer = 0;
        float timeToShoot = 5000;
        float timeToCloseLoader = 6000;
        public static bool launched = false;
        public static bool closeLoader = false;

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
            pressed = false; showBoard = true; pressed2 = false;
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

            font = Content.Load<SpriteFont>("MenuFont");
            smoke = Content.Load<Texture2D>("smoke");

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Initial camera load
            camera = new ChaseCamera();
            cameraView = CameraView.OVERVIEW;
            Player player = playerManager.GetHumanPlayer();
            UpdatePlayerCamera(player);
            camera.Reset();

            collisionManager = new PlayerCollisionManager(Content, trackManager.track);
            collisionManager.InitializePlayers(playerManager.npcs, player);
            InitEffect();
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
                collisionManager.update(gameTime);
 
                // Update camera
                Player player = playerManager.GetHumanPlayer();
                UpdatePlayerCamera(player);
            }

            UpdateLoader(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkViolet);
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

            if (gameState == states.play)
            {
                if (showBoard)
                {
                    Vector2 startLeaderboard1 = new Vector2(graphics.PreferredBackBufferWidth - 280, 15);
                    Vector2 startLeaderboard2 = new Vector2(graphics.PreferredBackBufferWidth - 230, 200);

                    int count = 1;
                    //Leaderboards
                    spriteBatch.Draw(smoke, new Vector2(990, 0), new Rectangle(0, 0, 2000, 2000), Color.FromNonPremultiplied(148, 0, 211, 150));

                    spriteBatch.DrawString(font, " - SCORES -", startLeaderboard1 + new Vector2(50, 0), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, " - RANKINGS - ", startLeaderboard2, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                    foreach (Player p in collisionManager.GetLeadersPoints())
                    {
                        //SCORES
                        spriteBatch.DrawString(font, p.name, startLeaderboard1 + new Vector2(0, count * 30), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(font, p.score.ToString(), startLeaderboard1 + new Vector2(170, count * 30), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        ++count;
                    }
                    count = 1;
                    foreach (Player p in collisionManager.GetLeadersRank())
                    {
                        //RANKS
                        spriteBatch.DrawString(font, p.name, startLeaderboard2 + new Vector2(0, count * 30), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(font, count.ToString(), startLeaderboard2 + new Vector2(170, count * 30), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        ++count;
                    }

                    Vector2 mapStart = new Vector2(graphics.PreferredBackBufferWidth - 230, 200);
                    //Minimap
                    foreach (Player p in collisionManager.GetLeadersRank())
                    {

                    }
                }
            }                  
            if (gameState == states.pause)
            {
                spriteBatch.Draw(smoke, new Vector2(0, 0), new Rectangle(0, 0, 2000, 2000), Color.FromNonPremultiplied(155, 155, 155, 155));            
                spriteBatch.DrawString(font, "PAUSED", new Vector2((graphics.PreferredBackBufferWidth / 2) - 25, graphics.PreferredBackBufferHeight / 2), Color.White);
                spriteBatch.DrawString(font, "(i)Instructions", new Vector2((graphics.PreferredBackBufferWidth / 2) - 25, graphics.PreferredBackBufferHeight / 2 + 30), Color.White);
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
                    Color.FromNonPremultiplied(155, 155, 155, 220)); 
                spriteBatch.DrawString(font, "INSTRUCTIONS", new Vector2((graphics.PreferredBackBufferWidth / 2) - 85, graphics.PreferredBackBufferHeight / 10 + 22), Color.White);

                spriteBatch.DrawString(font, " - Free Camera -", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 25), Color.White);
                spriteBatch.DrawString(font, "Use W/A/S/D to rotate camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 65), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Use Q/E to move forward and backward", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 85), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "X to reset camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 105), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Use I/J/K/L to move camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 125), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Use 8/9/0 for 1st person/3rd person/overview camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 145), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                
                spriteBatch.DrawString(font, "Use Up/Down/Left/Right to move Ball", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 185), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Space Next/Pause", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 205), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
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

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Down) || 
                keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Right))
            {                
                //UpdateCameraAngle(keyboardState);
            }

            //accepts all th input and reacts to it
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                //trackManager.track.springLevel -= 0.01f;
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
            if (keyboardState.IsKeyDown(Keys.D6))
            {
                camAngle -= 0.05f;
            }
            if (keyboardState.IsKeyDown(Keys.D7))
            {
                camAngle += 0.05f;
            }
            if (keyboardState.IsKeyDown(Keys.D8))
            {
                cameraView = CameraView.FIRST_PERSON;
            }
            if (keyboardState.IsKeyDown(Keys.D9))
            {
                cameraView = CameraView.THIRD_PERSON;
            }
            if (keyboardState.IsKeyDown(Keys.D0) || keyboardState.IsKeyDown(Keys.X))
            {
                cameraView = CameraView.OVERVIEW;
                view = Matrix.CreateLookAt(new Vector3(20, 50, 70f), new Vector3(20, 50, 0), Vector3.UnitY);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 16 / 9, 1, 200);
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
                if (cameraView == CameraView.OVERVIEW)
                {
                    angleY = 0.01f;
                    view = view * Matrix.CreateRotationY(angleY);
                }
                else
                {
                    camAngle += 0.05f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (cameraView == CameraView.OVERVIEW)
                {
                    angleY = -0.01f;
                    view = view * Matrix.CreateRotationY(angleY);
                }
                else
                {
                    camAngle -= 0.05f;
                }
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
                    previousKey = Keys.Space;
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
                else if (keyboardState.IsKeyDown(Keys.D1) && !pressed2)
                {
                    pressed2 = true;
                    showBoard = !showBoard;
                }
            }
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                pressed = false;
            }
            if (keyboardState.IsKeyUp(Keys.D1))
            {
                pressed2 = false;
            }
        }

        private void UpdateLoader(GameTime gameTime)
        {
            if ((!trackManager.track.springShot || !closeLoader) && gameState == states.play)
            {
                if (!trackManager.track.springShot)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    //trackManager.track.springLevel += 0.0001f;
                    playerManager.GetHumanPlayer().velocity = new Vector3(0, 0.001f, 0);
                    foreach (NpcPlayer n in playerManager.npcs)
                    {
                        n.velocity = new Vector3(0, 0.001f, 0);
                    }

                    if (timer > timeToShoot)
                    {
                        trackManager.track.springLevel = trackManager.track.maxSpringLevel;
                        playerManager.GetHumanPlayer().velocity += new Vector3(0, trackManager.track.maxSpringLevel * 5, 0);
                        foreach (NpcPlayer n in playerManager.npcs)
                        {
                            n.velocity += new Vector3(0, trackManager.track.maxSpringLevel * 5, 0);
                        }
                        trackManager.track.springShot = true;
                        launched = true;
                    }
                }
                else
                {
                    if (trackManager.track.springLevel > trackManager.track.stableSpringLevel)
                    {
                        trackManager.track.springLevel -= 0.05f;                        
                    }

                    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (timer > timeToCloseLoader)
                    {
                        closeLoader = true;
                    }
                }
            }
        }

        private void UpdatePlayerCamera(Player player)
        {
            Matrix transform = Matrix.Identity;            

            switch (cameraView)
            {
                case CameraView.FIRST_PERSON:
                    // Update view to a 1st person view according to the player's position      
                    //view = Matrix.CreateLookAt(player.position, player.position + new Vector3(0,1,0), Vector3.UnitZ) * Matrix.CreateRotationY(camAngle);
                    view = Matrix.CreateLookAt(player.position + Vector3.TransformNormal(new Vector3(0, -0.1f, 15f), transform), 
                        player.position, Vector3.UnitZ);// * Matrix.CreateRotationY(camAngle);
                    break;
                case CameraView.THIRD_PERSON:
                    // Update view to a 3rd person view according to the player's position
                    view = Matrix.CreateLookAt(player.position + Vector3.TransformNormal(new Vector3(0, -5, 2.5f), transform),
                        player.position, Vector3.UnitZ);
                    break;
                default:
                    // Update view to overview
                    //view = Matrix.CreateLookAt(new Vector3(20, 50, 70f), new Vector3(20, 50, 0), Vector3.UnitY);
                    break;
            }
        }

        private void UpdateCameraAngle(KeyboardState keyboardState)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            float targetAngle = 0;
            float angleIncrement = 0.05f;

            // Check to see which keys are pressed
            if (keyboardState.IsKeyDown(Keys.Up)){up = true;}
            if (keyboardState.IsKeyDown(Keys.Down)) { down = true; }
            if (keyboardState.IsKeyDown(Keys.Left)) { left = true; }
            if (keyboardState.IsKeyDown(Keys.Right)) { right = true; }

            // Determine the angle to target
            if (left)
            {
                targetAngle = 270;
            }

            if (right)
            {
                targetAngle = 90;
            }

            if(up){
                targetAngle = 0;
                if (up && left)
                {
                    targetAngle = -315;
                }

                if (up && right)
                {
                    targetAngle = 45;
                }
            }


            if (down)
            {
                targetAngle = 180;
                if (down && left)
                {
                    targetAngle = -225;
                }

                if (down && right)
                {
                    targetAngle = 135;
                }
            }

            // If the camera angle is set to the direction already
            if (camAngle == MathHelper.ToRadians(targetAngle))
            {
                return;
            }

            // Update the camera angle as needed
            if (camAngle > MathHelper.ToRadians(targetAngle))
            {
                camAngle -= angleIncrement;
            }

            if (camAngle < MathHelper.ToRadians(targetAngle))
            {
                camAngle += angleIncrement;
            }
        }
    }
}
