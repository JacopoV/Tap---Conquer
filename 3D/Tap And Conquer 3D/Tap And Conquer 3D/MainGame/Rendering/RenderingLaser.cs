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
using StateGameLayer;

namespace TapAndConquer3D
{
    public class RenderingLaser : RenderingFactory
    {

        Texture2D laserTexture;
        SpriteBatch spriteBatch;
          public StateGame stateGame;


        public RenderingLaser(ContentManager Content)
            : base(Content)
        {
            spriteBatch = Content.ServiceProvider.GetService(typeof(SpriteBatch)) as SpriteBatch;
             laserTexture = Content.Load<Texture2D>("TroopTexture/lasers");
             stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;

        }

        public void DrawLasers(GameState gameState)
        {

            base.DrawFactory(gameState, false);

            for (int i = 0; i < stateGame.lasers.Count; i++)
            {

                StateGameLayer.Laser lsr = stateGame.lasers[i];

                Vector3 nearPoint = new Vector3(lsr.Position.X, lsr.Position.Y, lsr.Position.Z);
                nearPoint = spriteBatch.GraphicsDevice.Viewport.Project(nearPoint, Projection, View, World);

                float finalX = nearPoint.X - (laserTexture.Width / 4) * 0.5f;
                float finalY = nearPoint.Y - (laserTexture.Width / 4) * 0.5f;


                Vector2 laserPosition = new Vector2(finalX, finalY);

                spriteBatch.Begin();

                spriteBatch.Draw(laserTexture, laserPosition,
                        new Rectangle(lsr.laserCurrentFrame.X * lsr.laserFrameSize.X,
                        lsr.laserCurrentFrame.Y * lsr.laserCurrentFrame.Y,
                        lsr.laserFrameSize.X, lsr.laserFrameSize.Y),
                        Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);

                spriteBatch.End();

            }

            

        }
    }
}
