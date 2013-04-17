using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PinballRacer.Players
{
    public class PlayerSpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        HumanPlayer human;
        public List<NpcPlayer> npcs;

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
            human.color = new Vector3(1, 0.4f, 0.3f);
            npcs = new List<NpcPlayer>();

            numberOfNpcs = 2;
            for (int i = 0; i < numberOfNpcs; ++i)
            {
                NpcPlayer p = new NpcPlayer();
                if (i == 0) p.color = new Vector3(1, 0.7f, 0.75f);
                else p.color = new Vector3(0.3f, 0.4f, 1);
                p.InitializeModel(ball);
                p.InitializePosition(new Vector3(15.0f +  i * 2, 10.0f, Player.RADIUS / 2), Vector3.Zero, Player.RADIUS, Vector3.Zero);
                npcs.Add(p);
            }

        }

        private void InitializeHumanPlayer()
        {
            human = new HumanPlayer();
            human.InitializeModel(ball);
            float scale = Player.RADIUS;
            Vector3 position = new Vector3(10.0f, 10.0f, Player.RADIUS / 2);
            Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);
            
            human.InitializePosition(position, direction, scale, rotation);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            ball = Game.Content.Load<Model>("testBall");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            human.Update(gameTime);
            foreach (Player p in npcs)
            {
                p.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            human.Draw(Game1.view, Game1.projection);
            foreach (Player p in npcs)
            {
                p.Draw(Game1.view, Game1.projection);
            }
        }

        public Player GetHumanPlayer()
        {
            return human;
        }
    }
}
