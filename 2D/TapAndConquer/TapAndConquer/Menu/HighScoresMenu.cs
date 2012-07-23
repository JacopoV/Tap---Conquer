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
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Input.Touch;
using System.Xml.Serialization;
using TapAndConquer.Resources;
using TapAndConquer.PersistentState;


namespace TapAndConquer.Menu
{
    public class HighScoresMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields

        SpriteBatch spriteBatch;
        Rectangle easy_rect, medium_rect, hard_rect, back_rect;
        Vector2 easy_pos, medium_pos, hard_pos;
        SpriteFont font36, font24;
        Texture2D back_btn;
        int modescore_to_show = 0;

        #endregion

        public HighScoresMenu(Game game)
            : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.Tap;
        }

        public override void Initialize()
        {
            spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font36 = Game.Content.Load<SpriteFont>("Fonts/Regular36");
            font24 = Game.Content.Load<SpriteFont>("Fonts/Regular24");
            back_btn = Game.Content.Load<Texture2D>("Textures/back");

            var midd_w = GraphicsDevice.Viewport.Width / 2;

            medium_rect = new Rectangle(midd_w - (int)(font36.MeasureString(Strings.Medium).X / 2.0f),
                400, (int)font36.MeasureString(Strings.Medium).X, (int)font36.MeasureString(Strings.Medium).Y);
            medium_pos = new Vector2(medium_rect.X, medium_rect.Y);

            easy_rect = new Rectangle(medium_rect.X - 60 - (int)font36.MeasureString(Strings.Easy).X, 
                400, (int)font36.MeasureString(Strings.Easy).X,
                (int)font36.MeasureString(Strings.Easy).Y);
            easy_pos = new Vector2(easy_rect.X, easy_rect.Y);

            hard_rect = new Rectangle(medium_rect.Right + 60,
                400, (int)font36.MeasureString(Strings.Hard).X, (int)font36.MeasureString(Strings.Hard).Y);
            hard_pos = new Vector2(hard_rect.X, hard_rect.Y);

            back_rect = new Rectangle(0, 0, back_btn.Width, back_btn.Height);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gs = TouchPanel.ReadGesture();
                if (gs.GestureType == GestureType.Tap)
                {
                    var pos = new Point((int)gs.Position.X, (int)gs.Position.Y);

                    if (easy_rect.Contains(pos))
                    {
                        modescore_to_show = 0;
                    }
                    else if (medium_rect.Contains(pos))
                    {
                        modescore_to_show = 1;
                    }
                    else if (hard_rect.Contains(pos))
                    {
                        modescore_to_show = 2;
                    }
                    else if (back_rect.Contains(pos))
                    {
                        if (Back != null) Back();
                    }
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                if (Back != null)
                    Back();

            base.Update(gameTime);
        }

        public event Action Back;
        public Vector2 vectorAdd3 = Vector2.One * 4.0f;

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/background"), Vector2.Zero, Color.White);

            spriteBatch.DrawString(font36, Strings.Easy, easy_pos + vectorAdd3, Color.Black * 0.5f);
            spriteBatch.DrawString(font36, Strings.Medium, medium_pos + vectorAdd3, Color.Black * 0.5f);
            spriteBatch.DrawString(font36, Strings.Hard, hard_pos + vectorAdd3, Color.Black * 0.5f);

            spriteBatch.DrawString(font36, Strings.Easy, easy_pos, 
                modescore_to_show == 0 ? Color.White : Color.White * 0.5f);
            spriteBatch.DrawString(font36, Strings.Medium, medium_pos, 
                modescore_to_show == 1 ? Color.White : Color.White * 0.5f);
            spriteBatch.DrawString(font36, Strings.Hard, hard_pos, 
                modescore_to_show == 2 ? Color.White : Color.White * 0.5f);

            spriteBatch.Draw(back_btn, Vector2.Zero, Color.White);

            switch (modescore_to_show)
            { 
                case 1:
                    for (int i = 0; i < HighScores.Current.mediumHighScores.Count; i++)
                        spriteBatch.DrawString(font24, HighScores.Current.mediumHighScores[i].ToString(),
                            new Vector2(120, 85 + i * 50), Color.White);
                    break;
                case 2:
                    for (int i = 0; i < HighScores.Current.hardHighScores.Count; i++)
                        spriteBatch.DrawString(font24, HighScores.Current.hardHighScores[i].ToString(),
                            new Vector2(120, 85 + i * 50), Color.White);
                    break;
                default:
                    for (int i = 0; i < HighScores.Current.easyHighScores.Count; i++)
                        spriteBatch.DrawString(font24, HighScores.Current.easyHighScores[i].ToString(),
                            new Vector2(120, 85 + i * 50), Color.White);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
