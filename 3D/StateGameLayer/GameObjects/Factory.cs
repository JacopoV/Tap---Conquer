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
    public class Factory : GameObject
    {
        public int maxElements;
        public bool underAttack;
        public bool selected = false;
        public bool reinforced = false;
        public bool selectionSound = false;
        public string namePlanet;
        public int textureType;
        public int scaleFactor;
        public float iLoveTurningAroundMyYAxis;

        public Factory() { }
        public Factory(int owner, Vector3 position, int maxElements, int area, int maxHitPoints, int damage, int defense, int shields, int textureType, string name, int scaleFactor)
            :base(position, owner, area, 0, maxHitPoints, damage, defense, shields)
        {
            if (ObjectType == 0)
                Elements = 0;
            else
                Elements = 1;
            this.maxElements = maxElements;
            underAttack = false;
            TimeLimit = 2.5f;
            RegenTime = 10;
            this.scaleFactor = scaleFactor;

            //calculate center
            Center = CurrentPos;

            namePlanet = name;
            float [] random = {0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f};
            
            Random r = new Random();
            iLoveTurningAroundMyYAxis = random[r.Next(7)];

            this.textureType = textureType;
        }

        public override int getModelNumber()
        {
            int textureNumber = 0;
            if (selected) textureNumber++;
            if (ObjectType == 1) textureNumber += 2; //2 blu, 3 blu ill
            if (ObjectType == 2) textureNumber += 4; //4 red, 5 red ill

            if (reinforced) textureNumber = 10;//blu rinforzo

            return textureNumber;
        }

        public int getSendableTroops(bool send)
        {
            int numberOfTroops = Elements;
            if (send && Elements >= 1)
            {
                Elements = 1;
            }
            return numberOfTroops - 1;
        }

        public void attack() 
        {
            underAttack = true;
            StartTime = 0;
        }

        public bool compareFactory(Factory toCompare)
        {
            return (Center == toCompare.Center);
        }

        public bool UnderAttack
        {
            set { underAttack = value; }
            get { return underAttack; }
        }

        public int MaxElements
        {
            set { maxElements = value; }
            get { return maxElements; }
        }

        public bool Selected
        {
            set { selected = value; }
            get { return selected; }
        }

        public bool Reinforced
        {
            set { reinforced = value; }
            get { return reinforced; }
        }

        public override void recalculateElements(int initialHP)
        {
            int deads = (int)((initialHP - HitPoints) / (float)MaxHitPoints * 0.66);
            Elements = Math.Max(Elements - deads, 0);
        }

        public float rot = 1;

        public float axisRotation()
        {
            rot += iLoveTurningAroundMyYAxis;
            if (rot > 360)
                rot = 1;
            return rot;
 
        }
    }
}
