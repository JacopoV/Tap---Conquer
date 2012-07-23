using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using StateGameLayer;

namespace TapAndConquer3D
{
    public class ExplosionState : LaserState
    {

        public StateGame stateGame;

        public ExplosionState(ContentManager Content)
            :base(Content)
        {

            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;

        }

        public void addExplosion(Vector3 explosionPos)
        {

            stateGame.explosions.Add(new StateGameLayer.Explosion(explosionPos));

        }

        public void update(float time)
        {

            base.update(time);

            /*
             * at every update, if explosions are > 0 manage explosions!
             */

            if (stateGame.explosions.Count > 0)
            {

                for (int i = 0; i < stateGame.explosions.Count; i++)
                {

                    StateGameLayer.Explosion exp = stateGame.explosions[i];

                    exp.time += time;
                    exp.partialTimeSprite += time;

                    if (exp.partialTimeSprite > exp.totalTimeSprite)
                    {
                        exp.ExplosionCurrentFrame.X += 1;
                        exp.partialTimeSprite = 0.0f;
                    }


                    if (exp.ExplosionCurrentFrame.X >= exp.ExplosionSheetSize.X)
                            {
                               exp.ExplosionCurrentFrame.X = 0;
                                exp.ExplosionCurrentFrame.Y += 1;
                                if (exp.ExplosionCurrentFrame.Y >= exp.ExplosionSheetSize.Y)
                                    exp.ExplosionCurrentFrame.Y = 0;
                            
                     }



                    if (stateGame.explosions[i].time > stateGame.explosions[i].timeExplosion)
                    {
                        /*
                         * remove explosion, time is up
                         */
                        stateGame.explosions.RemoveAt(i);
                    }
                 }

            }


        }
    }
}
