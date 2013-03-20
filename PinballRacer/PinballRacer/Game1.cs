using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PinballRacer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BasicEffect m_basicEffect;

        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 12f), Vector3.Zero, Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 600 / 600, 1, 200);
        Model PinballTable;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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

          //  DrawLines(view,projection, new Vector3(-100,0,0), new Vector3(100,0,0));

            // TODO: Add your drawing code here

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
    }
}
