using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PinballRacer.Players
{
    public class PlayerSpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        HumanPlayer human;
        List<NpcPlayer> npcs;

        Model ball;

        int numberOfNpcs;


        public PlayerSpriteManager(Game1 game)
            : base(game)
        {
            base.Initialize();            
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeHumanPlayer();

            npcs = new List<NpcPlayer>();

            numberOfNpcs = 4;
            for (int i = 0; i < numberOfNpcs; ++i)
            {
                npcs.Add(new NpcPlayer());
            }

        }

        private void InitializeHumanPlayer()
        {
            human = new HumanPlayer();
            human.InitializeModel(ball);

            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
            float scale = 10.0f;
            float rotation = 0.0f;
            
            human.InitializePlayer(position, direction, scale, rotation);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            ball = Game.Content.Load<Model>("ball");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            human.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            human.Draw(Game1.view, Game1.projection);

        }
    }
}
