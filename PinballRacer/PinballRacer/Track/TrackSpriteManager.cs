using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PinballRacer.Track
{
    public class TrackSpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;        

        //  Models        

        //  Track elements
        public RaceTrack track;

        public TrackSpriteManager(Game1 game)
            : base(game)
        {
            base.Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            track = new RaceTrack(Game.Content);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Game.Content.Load<Model>("cube");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);

            track.Draw(Game1.view, Game1.projection);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.End();
            
        }

        
        
    }
}
