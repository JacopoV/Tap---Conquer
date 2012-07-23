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

namespace TapAndConquer
{
    class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {

        SpriteBatch spriteBatch;
        Texture2D menuBackground;
        Texture2D playButtonImage;
        Texture2D quitButtonImage;
        Rectangle playRectangle;
        Rectangle quitRectangle;

        public event Action playGame;
        public event Action quit;

        public MainMenu(Game game)
        : base(game)
        {

            TouchPanel.EnabledGestures = GestureType.Tap;
             playRectangle = new Rectangle(game.GraphicsDevice.Viewport.Width/2 - 130, game.GraphicsDevice.Viewport.Height/2 - 40, 250, 100);
             quitRectangle = new Rectangle(game.GraphicsDevice.Viewport.Width/2 - 130, game.GraphicsDevice.Viewport.Height/2 + 70, 250, 100);
        }

        public override void Initialize()
        {
            spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            base.Initialize();
        }

        public void loadContent()
        {
            menuBackground = Game.Content.Load<Texture2D>("bkg");
            playButtonImage = Game.Content.Load<Texture2D>("play");
            quitButtonImage = Game.Content.Load<Texture2D>("quit");
        }

        public override void Update(GameTime gameTime)
        {

            while (TouchPanel.IsGestureAvailable)
            {
                var gs = TouchPanel.ReadGesture();
                if (gs.GestureType == GestureType.Tap || Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    var pos = new Point((int)gs.Position.X, (int)gs.Position.Y);
                    if (playRectangle.Contains(pos))
                    {
                        if (playRectangle != null) playGame();
                    }
                    else if (quitRectangle.Contains(pos))
                    {
                        if (quitRectangle != null) quit();
                    }
                }
            }

#if WINDOWS
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var pos = new Point((int)Mouse.GetState().X, (int)Mouse.GetState().Y);
                if (playRectangle.Contains(pos))
                {
                    if (playRectangle != null) playGame();
                }
                else if (quitRectangle.Contains(pos))
                {
                    if (quitRectangle != null) quit();
                }
            }
#endif
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            
            spriteBatch.Draw(playButtonImage, playRectangle, Color.White);
            spriteBatch.Draw(quitButtonImage, quitRectangle, Color.White);

            spriteBatch.End();
        }

    }
}
