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

namespace StateGameLayer
{
    public class Laser
    {

        public Point laserFrameSize = new Point(64, 64);
        public Point laserCurrentFrame = new Point(0, 0);
        public Point laserSheetSize = new Point(16, 4);

        public bool soundLaser;
        public Vector3 Position;
        public Troop laserTroop;
        public float totalTimeSprite;
        public float partialTimeSprite;

        public Laser() { }
        public Laser(Vector3 pos, Troop troop)
        {
            Position = pos;
            laserTroop = troop;
            totalTimeSprite = 0.01f;
            partialTimeSprite = 0.0f;
            soundLaser = true;
        
        }

     }
}
