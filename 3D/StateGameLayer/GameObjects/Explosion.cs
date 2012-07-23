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
    public class Explosion 
    {

        public Point ExplosionFrameSize = new Point(64, 64);
        public Point ExplosionCurrentFrame = new Point(0, 0);
        public Point ExplosionSheetSize = new Point(16, 4);


        public bool soundExplosion;
        public Vector3 Position;
        public float time;
        public float timeExplosion;
        public float totalTimeSprite;
        public float partialTimeSprite;


        public Explosion() { }
        public Explosion(Vector3 pos)
        {

            Position = pos;
            time = 0.0f;
            timeExplosion = 0.5f;
            totalTimeSprite = 0.01f;
            partialTimeSprite = 0.0f;
            soundExplosion = true;

        }

    }
}
