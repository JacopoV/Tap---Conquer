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

namespace TapAndConquer
{
    public class RenderingBackground
    {
        private Texture2D terrain;

        public RenderingBackground(ContentManager Content)
        {
            terrain = Content.Load<Texture2D>("GameTexture/terrain");
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        { 
            spriteBatch.Draw(terrain, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
        }
    }
}
