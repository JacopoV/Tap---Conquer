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
using TapAndConquer3D.Resources;
using TapAndConquer3D.PersistentState;


namespace TapAndConquer3D.Menu
{
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Rectangle res_rect, new_gam_rect, hig_sc_rect, tooltips_rec, back_rect;
        Vector2 res_pos, new_gam_pos, hig_sc_pos, tooltips_pos;
        SpriteFont font36, font14;
        Texture2D back_btn;

        public MainMenu(Game game)
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
            font14 = Game.Content.Load<SpriteFont>("Fonts/Regular14");
            back_btn = Game.Content.Load<Texture2D>("Textures/back");

            var midd_w = GraphicsDevice.Viewport.Width / 2;

            res_rect = new Rectangle(midd_w - (int)(font36.MeasureString(Strings.ResumeGame).X / 2.0f),
                120, (int)font36.MeasureString(Strings.ResumeGame).X, (int)font36.MeasureString(Strings.ResumeGame).Y);
            res_pos = new Vector2(res_rect.X, res_rect.Y);

            new_gam_rect = new Rectangle(midd_w - (int)(font36.MeasureString(Strings.NewGame).X / 2.0f),
                200, (int)font36.MeasureString(Strings.NewGame).X, (int)font36.MeasureString(Strings.NewGame).Y);
            new_gam_pos = new Vector2(new_gam_rect.X, new_gam_rect.Y);

            hig_sc_rect = new Rectangle(midd_w - (int)(font36.MeasureString(Strings.HighScores).X / 2.0f),
                280, (int)font36.MeasureString(Strings.HighScores).X, (int)font36.MeasureString(Strings.HighScores).Y);
            hig_sc_pos = new Vector2(hig_sc_rect.X, hig_sc_rect.Y);

            tooltips_rec = new Rectangle(GraphicsDevice.Viewport.Width - (int)font14.MeasureString(Strings.ToggleTooltips + "AAAAAA").X, 
                440, (int)font14.MeasureString(Strings.ToggleTooltips).X, (int)font14.MeasureString(Strings.ToggleTooltips+"AAAAA").Y);
            tooltips_pos = new Vector2(tooltips_rec.X, tooltips_rec.Y);

            tooltips_rec.X -= 30;
            tooltips_rec.Y -= 30;
            tooltips_rec.Width += 60;
            tooltips_rec.Height += 60;

            back_rect = new Rectangle(0, 0, back_btn.Width, back_btn.Height);

            base.LoadContent();
        }

        string savePath = "current.sav";
        bool saveGameExists { get { return IS.FileExists(savePath); } }
#if WINDOWS_PHONE
        IsolatedStorageFile IS = IsolatedStorageFile.GetUserStoreForApplication();
#endif
#if WINDOWS
        IsolatedStorageFile IS = IsolatedStorageFile.GetUserStoreForDomain();
#endif

#if WINDOWS_PHONE
        public override void Update(GameTime gameTime)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gs = TouchPanel.ReadGesture();
                if (gs.GestureType == GestureType.Tap)
                {
                    var pos = new Point((int)gs.Position.X, (int)gs.Position.Y);
                    if (res_rect.Contains(pos))
                    {
                        if (ResumeGame != null && saveGameExists) ResumeGame();
                    }
                    else if (new_gam_rect.Contains(pos))
                    {
                        if (NewGame != null) NewGame();
                    }
                    else if (tooltips_rec.Contains(pos))
                    {
                        ToggleTooltips();
                    }
                    else if (hig_sc_rect.Contains(pos))
                    {
                        if (HighScores != null) HighScores();
                    }
                    else if (back_rect.Contains(pos))
                    {
                        if (Exit != null) Exit();
                    }
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                if (Exit != null) Exit();

            base.Update(gameTime);
        }
#endif

#if WINDOWS
        public override void Update(GameTime gameTime)
        {
            
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    var pos = new Point((int)Mouse.GetState().X, (int)Mouse.GetState().Y);
                    if (res_rect.Contains(pos))
                    {
                        if (ResumeGame != null && saveGameExists) ResumeGame();
                    }
                    else if (new_gam_rect.Contains(pos))
                    {
                        if (NewGame != null) NewGame();
                    }
                    else if (tooltips_rec.Contains(pos))
                    {
                        ToggleTooltips();
                    }
                    else if (hig_sc_rect.Contains(pos))
                    {
                        if (HighScores != null) HighScores();
                    }
                    else if (back_rect.Contains(pos))
                    {
                        if (Exit != null) Exit();
                    }
                }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                if (Exit != null) Exit();

            base.Update(gameTime);
        }
#endif

        private void ToggleTooltips()
        {
            GameOptions.ShowTooltips = !GameOptions.ShowTooltips;
        }

        public event Action ResumeGame;
        public event Action NewGame;
        public event Action HighScores;
        public event Action Exit;
        public Vector2 vectorAdd3 = Vector2.One * 4.0f;

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(Game.Content.Load<Texture2D>("MenuTexture/bkg"), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            spriteBatch.DrawString(font36, Strings.ResumeGame, res_pos + vectorAdd3, Color.Black * 0.5f);
            spriteBatch.DrawString(font36, Strings.NewGame, new_gam_pos + vectorAdd3, Color.Black * 0.5f);
            //spriteBatch.DrawString(font14, Strings.ToggleTooltips + (GameOptions.ShowTooltips ? Strings.On : Strings.Off),
            //  tooltips_pos + Vector2.One * 2.0f, Color.Black * 0.5f);
            spriteBatch.DrawString(font36, Strings.HighScores, hig_sc_pos + vectorAdd3, Color.Black * 0.5f);

            spriteBatch.DrawString(font36, Strings.ResumeGame, res_pos, saveGameExists ? Color.White : Color.White * 0.7f);
            spriteBatch.DrawString(font36, Strings.NewGame, new_gam_pos, Color.White);
            //spriteBatch.DrawString(font14, Strings.ToggleTooltips + (GameOptions.ShowTooltips ? Strings.On : Strings.Off),
            //  tooltips_pos, Color.White);
            spriteBatch.DrawString(font36, Strings.HighScores, hig_sc_pos, Color.White);

            spriteBatch.Draw(back_btn, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
