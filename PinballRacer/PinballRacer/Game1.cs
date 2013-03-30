using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PinballRacer.Player;
using PinballRacer.Track;

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

        public static Matrix view = Matrix.CreateLookAt(new Vector3(10, 10, 24f), Vector3.Zero, Vector3.UnitY);
        public static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 800 / 600, 1, 100);

        Model PinballTable;

        //Camera attributes
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
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
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

            cameraMotion(keyboardState);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            #region ResetGraphic
            ResetGraphic();
            #endregion

            #region render 3D
            BeginRender3D();
            #endregion

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            //spriteBatch.End();

            base.Draw(gameTime);
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
            if (keyboardState.IsKeyDown(Keys.X))
            {
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                angleX = 0.009f;
                view = view * Matrix.CreateRotationX(angleX);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                angleX = -0.009f;
                view = view * Matrix.CreateRotationX(angleX);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                angleY = 0.009f;
                view = view * Matrix.CreateRotationY(angleY);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                angleY = -0.009f;
                view = view * Matrix.CreateRotationY(angleY);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
            if (keyboardState.IsKeyDown(Keys.J))
            {
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
        }

        public void ResetGraphic()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

        }
        public void BeginRender3D()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }


    }
}
