using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StateGameLayer;
using Microsoft.Xna.Framework.Content;

namespace TapAndConquer3D
{
    public class LaserState
    {

        public StateGame stateGame;

        public LaserState(ContentManager Content)
        {
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
        }

        public void addLaser(Vector3 laserPos, StateGameLayer.Troop toAdd)
        {
            bool insert = true;

            for (int i = 0; i < stateGame.lasers.Count; i++)
            {

                if (stateGame.lasers[i].laserTroop == toAdd)
                    insert = false;
            }

            if (insert)
            {
                stateGame.lasers.Add(new StateGameLayer.Laser(laserPos, toAdd));
            }

        }


        public void removeLaser(StateGameLayer.Troop toRemove)
        {
            for (int i = 0; i < stateGame.lasers.Count; i++)
            {


                if (stateGame.lasers[i].laserTroop == toRemove)
                {
                    stateGame.lasers.RemoveAt(i);

                }

            }
        }


        public void update(float time)
        {

            /*
             * at every update, if lasers are > 0 FIRE IN THE HOLE!
             */

            if (stateGame.lasers.Count > 0)
            {

                for (int i = 0; i < stateGame.lasers.Count; i++)
                {

                    StateGameLayer.Laser lsr = stateGame.lasers[i];

                    
                    lsr.partialTimeSprite += time;

                    if (lsr.partialTimeSprite > lsr.totalTimeSprite)
                    {
                        lsr.laserCurrentFrame.X += 1;
                        lsr.partialTimeSprite = 0.0f;
                    }


                    if (lsr.laserCurrentFrame.X >= lsr.laserSheetSize.X)
                    {
                        lsr.laserCurrentFrame.X = 0;
                        lsr.laserCurrentFrame.Y += 1;
                        if (lsr.laserCurrentFrame.Y >= lsr.laserSheetSize.Y)
                            lsr.laserCurrentFrame.Y = 0;

                    }
                }

            }


        }
    }
}
