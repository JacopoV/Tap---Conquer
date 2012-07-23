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
    public class RenderingExplosion : RenderingTroop
    {

        public StateGame stateGame; 

        public Texture2D explosionTexture;
        public SpriteBatch spriteBatch;

        public RenderingExplosion(ContentManager Content)
            : base(Content)
        {

            spriteBatch = Content.ServiceProvider.GetService(typeof(SpriteBatch)) as SpriteBatch;
            explosionTexture = Content.Load<Texture2D>("TroopTexture/explosion");
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame; 
           
        }

        public void DrawExplosions(GameState gameState)
        {

    base.DrawTroop(gameState,false);
 	
 	for (int i = 0; i < stateGame.explosions.Count; i++)
 	{
 	
 	StateGameLayer.Explosion exp = stateGame.explosions[i];
 	
 	Vector3 nearPoint = new Vector3(exp.Position.X, exp.Position.Y, exp.Position.Z);
 	nearPoint = spriteBatch.GraphicsDevice.Viewport.Project(nearPoint, Projection, View, World);
 	
 	
 	Vector2 explosionPosition = new Vector2(nearPoint.X - (explosionTexture.Width / 4) * 0.5f, nearPoint.Y - (explosionTexture.Width / 4) * 0.5f);
 	
 	spriteBatch.Begin();
 	
 	spriteBatch.Draw(explosionTexture, explosionPosition,
 	new Rectangle(exp.ExplosionCurrentFrame.X * exp.ExplosionFrameSize.X,
 	exp.ExplosionCurrentFrame.Y * exp.ExplosionCurrentFrame.Y,
 	exp.ExplosionFrameSize.X, exp.ExplosionFrameSize.Y),
 	Color.White, 0, Vector2.Zero,
 	1, SpriteEffects.None, 1);
 	
 	spriteBatch.End(); 

            }


        }
    }
}
