using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using TapAndConquer.GameObjects;


namespace TapAndConquer
{
    public class RenderingFactory : RenderingBackground
    {
        protected static Texture2D red;
        protected static Texture2D blue;
        protected static Texture2D green;
        protected static Texture2D light_red;
        protected static Texture2D light_blue;
        protected static Texture2D light_green;
        protected static Texture2D reinforce;

        private static SpriteFont font;

        public RenderingFactory(ContentManager Content)
            : base(Content)
        {
            red = Content.Load<Texture2D>("FactoryTexture/red_factory");
            blue = Content.Load<Texture2D>("FactoryTexture/blue_factory");
            green = Content.Load<Texture2D>("FactoryTexture/free_factory");
            light_red = Content.Load<Texture2D>("FactoryTexture/light_red_factory");
            light_blue = Content.Load<Texture2D>("FactoryTexture/light_blue_factory");
            light_green = Content.Load<Texture2D>("FactoryTexture/light_free_factory");
            reinforce = Content.Load<Texture2D>("FactoryTexture/sword");

            font = Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        public void DrawFactory(SpriteBatch spriteBatch, GameState gameState)
        {

            base.DrawBackground(spriteBatch);

            List<Factory> factories = gameState.getFactories();

            for (int i = 0; i < factories.Count; i++)
            {
                if (factories[i].getTextureNumber() == 0)
                {
                    spriteBatch.Draw(green, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 1)
                {
                    spriteBatch.Draw(light_green, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 2)
                {
                    spriteBatch.Draw(blue, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 3)
                {
                    spriteBatch.Draw(light_blue, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 4)
                {
                    spriteBatch.Draw(red, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 5)
                {
                    spriteBatch.Draw(light_red, factories[i].CurrentPos, Color.White);
                }
                else if (factories[i].getTextureNumber() == 10)
                {
                    spriteBatch.Draw(reinforce, factories[i].CurrentPos, Color.White);
                }

                spriteBatch.DrawString(font, factories[i].Elements.ToString() + "/" + factories[i].MaxElements.ToString(), factories[i].CurrentPos, Color.Yellow);
            }
        }
    }
}
