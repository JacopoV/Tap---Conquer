using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TapAndConquer.Resources;

namespace TapAndConquer
{
    public class TrialComponent : DrawableGameComponent
    {
        #region Fields

        private SpriteBatch spriteBatch;

        private float elapsed_time, available_time, time_needed_to_read = 7, next_show, message_alpha = 1.5f;
        private string trial_file_name, message_text = "", first_line, second_line;
        private bool show_message = false;
        private Texture2D message_bg, white_pixel;
        private SpriteFont font36, font24;
        private Vector2 bg_pos, first_line_pos, second_line_pos;

        #endregion

        public TrialComponent(Game game, SpriteBatch spriteBatch, int available_time, string app_id)
            : base(game)
        {
            this.available_time = available_time;
            this.spriteBatch = spriteBatch;

            trial_file_name = app_id + "_trial";

            elapsed_time = GetTrialElapsedTime();
            next_show = ((int)elapsed_time / 60) * 60;

            DrawOrder = 1000;

            Strings.Culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        public override void Initialize()
        {
            Game.Exiting += new EventHandler<EventArgs>(Game_Exiting);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font36 = Game.Content.Load<SpriteFont>("Fonts/Regular36");
            font24 = Game.Content.Load<SpriteFont>("Fonts/Regular24");
            message_bg = Game.Content.Load<Texture2D>("Textures/trial_bg");
            white_pixel = Game.Content.Load<Texture2D>("Textures/whitePixel");

            bg_pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2.0f - message_bg.Width / 2.0f,
                Game.GraphicsDevice.Viewport.Height / 2.0f - message_bg.Height / 2.0f);

            var isLandscape = GraphicsDevice.Viewport.Width > GraphicsDevice.Viewport.Height;

            first_line = Strings.TrialExpired_l1;
            second_line = Strings.TrialExpired_l2;

            first_line_pos = new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font24.MeasureString(Strings.TrialExpired_l1).X / 2.0f,
                GraphicsDevice.Viewport.Height / 2.0f + 10);
            second_line_pos = new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font24.MeasureString(Strings.TrialExpired_l2).X / 2.0f,
                GraphicsDevice.Viewport.Height / 2.0f + 45);

            if (isLandscape)
            {
                first_line += " " + Strings.TrialExpired_l2;
                second_line = "";

                first_line_pos.X = GraphicsDevice.Viewport.Width / 2.0f - font24.MeasureString(first_line).X / 2.0f;
            }

            base.LoadContent();
        }

        protected override void Dispose(bool disposing)
        {
            SaveTrialState();

            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            elapsed_time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsed_time >= available_time)
            {
                if (elapsed_time > available_time + time_needed_to_read ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    Game.Exit();
            }
            else
            {
                if (message_alpha <= 0)
                {
                    message_alpha = 1.5f;
                    show_message = false;
                }

                if (show_message)
                {
                    message_alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (elapsed_time > next_show)
                {
                    show_message = true;
                    message_text = Strings.Trial + " -" + ((int)(available_time - next_show) / 60) + "'";
                    next_show = Math.Min(next_show + 60, available_time);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (elapsed_time >= available_time)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(white_pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height), Color.Black * 0.8f);

                spriteBatch.DrawString(font36, Strings.TrialExpired,
                    new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font36.MeasureString(Strings.TrialExpired).X / 2.0f, 150) +
                    Vector2.One * 2.0f, Color.Red);

                spriteBatch.DrawString(font36, Strings.TrialExpired,
                    new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font36.MeasureString(Strings.TrialExpired).X / 2.0f, 150),
                    Color.White);

                spriteBatch.DrawString(font24, first_line, first_line_pos + Vector2.One * 2.0f, Color.Red);
                spriteBatch.DrawString(font24, first_line, first_line_pos, Color.White);

                spriteBatch.DrawString(font24, second_line, second_line_pos + Vector2.One * 2.0f, Color.Red);
                spriteBatch.DrawString(font24, second_line, second_line_pos, Color.White);

                spriteBatch.End();
            }
            else
            {
                if (show_message)
                {
                    spriteBatch.Begin();

                    spriteBatch.Draw(message_bg, bg_pos, Color.White * Math.Min(1.0f, message_alpha));

                    spriteBatch.DrawString(font36, message_text,
                        new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font36.MeasureString(message_text).X / 2.0f,
                        GraphicsDevice.Viewport.Height / 2.0f - font36.MeasureString(message_text).Y / 2.0f) + Vector2.One * 2.0f,
                        Color.White * Math.Min(1.0f, message_alpha));

                    spriteBatch.DrawString(font36, message_text,
                        new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font36.MeasureString(message_text).X / 2.0f,
                        GraphicsDevice.Viewport.Height / 2.0f - font36.MeasureString(message_text).Y / 2.0f),
                        Color.Red * Math.Min(1.0f, message_alpha));

                    spriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }

        #region Save & Load

        private static Func<StreamReader, float> read_saved_time = reader =>
        {
            var e_time = float.Parse(reader.ReadToEnd());

            return e_time;
        };

        private static Action<StreamWriter, float> write_state = (writer, elapsed_time) => writer.Write(elapsed_time);

        public void SaveTrialState()
        {
            using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                appStorage.DeleteFile(trial_file_name);
                using (var streamFile = appStorage.OpenFile(trial_file_name, FileMode.OpenOrCreate))
                using (var writer = new StreamWriter(streamFile))
                    write_state(writer, Math.Min(elapsed_time, available_time));
            }
        }

        public float GetTrialElapsedTime()
        {
            using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var streamFile = appStorage.OpenFile(trial_file_name, FileMode.OpenOrCreate))
                {
                    try
                    {
                        using (var reader = new StreamReader(streamFile))
                            return read_saved_time(reader);
                    }
                    catch (Exception) { return 0; }
                }
            }
        }

        #endregion

        #region Exit Event Handler

        public void Game_Exiting(object sender, EventArgs e)
        {
            SaveTrialState();
        }

        #endregion
    }
}
