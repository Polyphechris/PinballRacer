using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PinballRacer.Players
{
    public class PlayerSpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        HumanPlayer human;
        List<NpcPlayer> npcs;

        int numberOfNpcs;


        public PlayerSpriteManager(Game1 game)
            : base(game)
        {
            base.Initialize();            
        }

        public override void Initialize()
        {
            base.Initialize();
            human = new HumanPlayer();
            npcs = new List<NpcPlayer>();

            numberOfNpcs = 4;
            for (int i = 0; i < numberOfNpcs; ++i)
            {
                npcs.Add(new NpcPlayer());
            }

        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
