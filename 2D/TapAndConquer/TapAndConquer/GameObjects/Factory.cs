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


namespace TapAndConquer.GameObjects
{
    public class Factory : GameObject
    {
        private int maxElements;
        private bool underAttack;
        private bool selected = false;
        private bool reinforced = false;

        private const int width = 100;
        private const int height = 100;

        public Factory(int owner, Vector2 position, int maxElements)
            :base(position, owner, 0)
        {
            if (ObjectType == 0)
                Elements = 0;
            else
                Elements = 1;
            this.maxElements = maxElements;
            underAttack = false;
            TimeLimit = 1;

            Radius = (int)(Height / 2);
            Center = new Vector2(CurrentPos.X + Width * 0.5f, CurrentPos.Y + Height * 0.5f);
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public override int getTextureNumber()
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

    }
}
