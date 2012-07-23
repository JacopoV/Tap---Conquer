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
    public class RenderingTroop : RenderingFactory
    {

        private static Texture2D red_tank;
        private static Texture2D blue_tank;
        private static Texture2D battle;
        private static SpriteFont font;


        public RenderingTroop(ContentManager Content)
            : base(Content)
        {
        
            red_tank = Content.Load<Texture2D>("TroopTexture/red_tank");
            blue_tank = Content.Load<Texture2D>("TroopTexture/blue_tank");
            battle = Content.Load<Texture2D>("TroopTexture/war");
            font = Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        protected Vector2 TankDimension
        {
            set { }

            get 
            {
                return new Vector2(red_tank.Width, red_tank.Height);
            }
        }


        public void DrawTroop(SpriteBatch spriteBatch, GameState gameState)
        {

            base.DrawFactory(spriteBatch, gameState);

            List<Troop> troops = gameState.getTroops();

            for(int i = 0; i < troops.Count; i++)
            {
                if (troops[i].Fighting)
                {
                    Vector2 troopOrigin = new Vector2(battle.Width * 0.5f, battle.Height * 0.5f);
                    spriteBatch.Draw(battle, troops[i].CurrentPos, null, Color.White, troops[i].Rotation, troopOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else if (troops[i].ObjectType == 1)
                {
                    Vector2 troopOrigin = new Vector2(blue_tank.Width * 0.5f, blue_tank.Height * 0.5f);
                    spriteBatch.Draw(blue_tank, troops[i].CurrentPos, null, Color.White, troops[i].Rotation, troopOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    Vector2 troopOrigin = new Vector2(red_tank.Width * 0.5f, red_tank.Height * 0.5f);
                    spriteBatch.Draw(red_tank, troops[i].CurrentPos, null, Color.White, troops[i].Rotation, troopOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }

                spriteBatch.DrawString(font, troops[i].Elements.ToString(), troops[i].CurrentPos, Color.Yellow);
            }

        }

    }
}
