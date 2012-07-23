using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TapAndConquer.Resources;


namespace TapAndConquer
{
    public enum AppBusinessType
    {
        Normal,
        Donation
    }

    public class LogoScreen : DrawableGameComponent
    {
        #region Fields

        public event Action OnLogoAppeared;

        private Rectangle texture_rectangle;
        private SpriteBatch sprite_batch;
        private SpriteFont font;
        private AppBusinessType type;
        private bool isTrial;
        private Texture2D logo;
        private Vector2 first_line, second_line, third_line, fourth_line;
        private string line_one, line_two, line_three, line_four;

        private string logo_path;
        private float alpha, time_to_wait;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="game">The game istance</param>
        /// <param name="OnLogoAppeared">Method to invoke once the logo is appeared</param>
        /// <param name="logo_path">Path of the texture, used to load the logo</param>
        public LogoScreen(Game game, Action OnLogoAppeared, string logo_path)
            : base(game)
        {
            alpha = 0.0f;
            this.logo_path = logo_path;
            this.OnLogoAppeared += new Action(OnLogoAppeared);
            this.type = AppBusinessType.Normal;

            isTrial = Guide.IsTrialMode;

            time_to_wait = type == AppBusinessType.Donation && isTrial ? 1.5f : 1.0f;
        }

        #endregion

        public override void Initialize()
        {
            sprite_batch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            logo = Game.Content.Load<Texture2D>(logo_path);

            var land = Game.GraphicsDevice.Viewport.Width > Game.GraphicsDevice.Viewport.Height ? true : false;

            texture_rectangle = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - logo.Width / 2,
                Game.GraphicsDevice.Viewport.Height / 2 - logo.Height / 2,
                logo.Width, logo.Height);

            font = Game.Content.Load<SpriteFont>("Fonts/Regular24");

            line_one = "";//land ? Lang.Land_First_Line : Lang.Por_First_Line;
            line_two = "";//land ? Lang.Land_Second_Line : Lang.Por_Second_Line;
            line_three = "";//land ? Lang.Land_Third_Line : Lang.Por_Third_Line;
            line_four = "";//land ? "" : Lang.Por_Fourth_Line;

            first_line = land ? new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_one).X / 2.0f, 330) : new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_one).X / 2.0f, 510);
            second_line = land ? new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_two).X / 2.0f, 360) : new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_two).X / 2.0f, 540);
            third_line = land ? new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_three).X / 2.0f, 390) : new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_three).X / 2.0f, 570);
            fourth_line = land ? new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_four).X / 2.0f, 420) : new Vector2(GraphicsDevice.Viewport.Width / 2.0f - font.MeasureString(line_four).X / 2.0f, 600);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            alpha += dt / 2;

            if (alpha > time_to_wait)
            {
                OnLogoAppeared();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sprite_batch.Begin();

            sprite_batch.Draw(logo, texture_rectangle, Color.White * (Math.Min(alpha, 1.0f)));

            if (isTrial && type == AppBusinessType.Donation)
            {
                sprite_batch.DrawString(font, line_one, first_line, Color.White);
                sprite_batch.DrawString(font, line_two, second_line, Color.White);
                sprite_batch.DrawString(font, line_three, third_line, Color.White);
                sprite_batch.DrawString(font, line_four, fourth_line, Color.White);
            }

            sprite_batch.End();

            base.Draw(gameTime);
        }
    }
}
