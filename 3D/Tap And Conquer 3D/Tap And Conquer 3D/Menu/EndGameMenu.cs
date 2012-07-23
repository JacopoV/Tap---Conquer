using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TapAndConquer3D.Menu
{
    class EndGameMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {

        SpriteBatch spriteBatch;
        Texture2D menuBackground;
        Texture2D menuButtonImage;
        Texture2D restartButtonImage;
        Texture2D winImage;
        Texture2D looseImage;
        Rectangle restartRectangle;
        Rectangle mainMenuRectangle;

        public bool resultGame;

        public event Action menu;
        public event Action restart;

        public EndGameMenu(Game game, bool win)
        : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.Tap;

            resultGame = win;

            spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            loadContent();

             restartRectangle = new Rectangle(game.GraphicsDevice.Viewport.Width - 400, game.GraphicsDevice.Viewport.Height - 250, 300, 100);
             mainMenuRectangle = new Rectangle(game.GraphicsDevice.Viewport.Width - 400, game.GraphicsDevice.Viewport.Height - 140, 300, 100);
        }

        public void loadContent()
        {
            menuBackground = Game.Content.Load<Texture2D>("MenuTexture/bkg");
            menuButtonImage = Game.Content.Load<Texture2D>("MenuTexture/main_menu");
            restartButtonImage = Game.Content.Load<Texture2D>("MenuTexture/restart");
            winImage = Game.Content.Load<Texture2D>("MenuTexture/win");
            looseImage = Game.Content.Load<Texture2D>("MenuTexture/lose");
        }

        public override void Update(GameTime gameTime)
        {
#if WINDOWS_PHONE
            while (TouchPanel.IsGestureAvailable)
            {
                var gs = TouchPanel.ReadGesture();
                if (gs.GestureType == GestureType.Tap)
                {
                    var pos = new Point((int)gs.Position.X, (int)gs.Position.Y);
                    if (restartRectangle.Contains(pos))
                    {
                        if (restartRectangle != null) restart();
                    }
                    else if (mainMenuRectangle.Contains(pos))
                    {
                        if (mainMenuRectangle != null) menu();
                    }
                }
            }
#endif
#if WINDOWS
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    var pos = new Point((int)Mouse.GetState().X, (int)Mouse.GetState().Y);
                    if (restartRectangle.Contains(pos))
                    {
                        if (restartRectangle != null) restart();
                    }
                    else if (mainMenuRectangle.Contains(pos))
                    {
                        if (mainMenuRectangle != null) menu();
                    }
                }
#endif
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.Begin();

            if (resultGame)
                menuBackground = winImage;
            else
                menuBackground = looseImage;

            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            
            spriteBatch.Draw(restartButtonImage, restartRectangle, Color.White);
            spriteBatch.Draw(menuButtonImage, mainMenuRectangle, Color.White);



            spriteBatch.End();
        }

    }
}