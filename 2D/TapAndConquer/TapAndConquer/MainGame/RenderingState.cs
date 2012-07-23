using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;


namespace TapAndConquer
{
    public class RenderingState : RenderingTroop
  {

    public SpriteFont font;
    public SpriteFont fontInfo;
    private Texture2D arrow;
    private Texture2D timeRectTexture;

    public RenderingState(ContentManager Content)
        : base(Content)
    {
        arrow = Content.Load<Texture2D>("GameTexture/arrow");
        fontInfo = Content.Load<SpriteFont>("Fonts/ListOrderFont");
        timeRectTexture = Content.Load<Texture2D>("MenuTexture/timer");
    }

    public void Draw(SpriteBatch spriteBatch, GameState gameState)
        {
            base.DrawTroop(spriteBatch, gameState);
            
            if (gameState.TouchBegin)
            {
                Vector2 originMinusDestination = gameState.ArrowDestination - gameState.ArrowOrigin;
                float stretchFactor = (originMinusDestination).Length() / arrow.Width;
                double angle = Math.Atan2(originMinusDestination.Y, originMinusDestination.X);
                spriteBatch.Draw(arrow, gameState.ArrowOrigin + originMinusDestination / 2, null, Color.White, (float)angle, new Vector2(arrow.Width, arrow.Height) / 2, new Vector2(stretchFactor, 1), SpriteEffects.None, 0);
            }
            
            DrawInfo(spriteBatch, gameState.numberOrders(1), gameState.LastTurn);
        }

      public void DrawInfo(SpriteBatch spriteBatch, int numberOrders, float time)
        {
            int seconds = (int)time;
            int decimals = (int)((time - seconds)*10);
            spriteBatch.DrawString(fontInfo, "Order List:" + numberOrders.ToString() + "/3", new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - 250, spriteBatch.GraphicsDevice.Viewport.Height - 50),
                numberOrders==3 ? Color.Red : numberOrders==2 ? Color.Orange : Color.White);
            spriteBatch.DrawString(fontInfo, "Turn Time:" + seconds + "." + decimals + "s/5s", new Vector2(10, spriteBatch.GraphicsDevice.Viewport.Height - 50), Color.WhiteSmoke);
            
          spriteBatch.Draw(timeRectTexture, new Rectangle(10, spriteBatch.GraphicsDevice.Viewport.Height - 50, 400-(seconds*80+decimals*8), 10), Color.White);
        }

    
  }

}
