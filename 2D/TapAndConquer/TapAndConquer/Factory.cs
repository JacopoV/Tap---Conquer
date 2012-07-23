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


namespace TapAndConquer
{
    public class Factory : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private SpriteFont Font1;
        private int numTroops;
        private int troopsCap;
        private int type; // 0 -> neutral factory - 1 -> factory player 1 - 2 -> factory player 2
        private const float timeProduceTroop = 1; 
        private float lastTime;
        private Vector2 factoryOrigin;
        private Vector2 factoryCenter;
        private Texture2D factoryImage;
        private float radius;
        private bool underAttack;

        public Factory(Game game, int t, Vector2 fO, int cap)
            :base(game)
        {
            if (t == 0)
                numTroops = 0;
            else
                numTroops = 1;
            type = t;
            factoryOrigin = fO;
            lastTime = 0;
            troopsCap = cap;
            underAttack = false;
        }

        public Vector2 getPosition()
        {
            return factoryOrigin;
        }

        public Vector2 getCenter()
        {
            return factoryCenter;
        }

        public int getType()
        {
            return type;
        }

        public float getRadius()
        {
            return radius;
        }

        public int getTroops()
        {
            return numTroops;
        }

        public int getSendableTroops(bool send)
        {
            int numberOfTroops = numTroops;
            if (send && numTroops >= 1)
            {
                numTroops = 1;
            }
            return numberOfTroops - 1;
        }

        public void attack() 
        {
            underAttack = true;
        }

        public void manageArrival(Troop troop)
        {
            underAttack = false;
            if (troop.getOwner() != type)
            {
                if (troop.getElements() > numTroops)
                {
                    numTroops = troop.getElements() - numTroops;
                    numTroops = Math.Min(numTroops, troopsCap);
                    type = troop.getOwner();
                    loadImages();
                }
                else
                {
                    numTroops -= troop.getElements();
                }
            }
            else
            {
                numTroops += troop.getElements();
                numTroops = Math.Min(numTroops, troopsCap);
            }
        }

        public void update(float time)
        {
            if(!underAttack)
                lastTime += time;
            if (lastTime >= timeProduceTroop)
            {
                if (type != 0 && numTroops < troopsCap)
                {
                    numTroops++;
                    lastTime = 0;
                }
            }            
        }

        public void loadImages()
        {
            if (type == 1)
            {
                factoryImage = Game.Content.Load<Texture2D>("blue_factory");
            }
            else if (type == 2)
            {
                factoryImage = Game.Content.Load<Texture2D>("red_factory");
            }
            else
            {
                factoryImage = Game.Content.Load<Texture2D>("free_factory");
            }

            //radius = factoryImage.Width/2;   // MODIFICA FOX SU CODICE ALFY
            radius = factoryImage.Width;
            factoryCenter = new Vector2(factoryOrigin.X + factoryImage.Width * 0.5f, factoryOrigin.Y + factoryImage.Height * 0.5f);

            Font1 = Game.Content.Load<SpriteFont>("FactoryFont");
        }

        public void loadImageSelected()
        {
            if (type == 1)
            {
                factoryImage = Game.Content.Load<Texture2D>("red_factory");
            }
            else if (type == 2)
            {
                factoryImage = Game.Content.Load<Texture2D>("blue_factory");
            }
            else
            {
                factoryImage = Game.Content.Load<Texture2D>("free_factory");
            }
        }

        public void loadImageWhenArrowOver()
        {
            if(type == 0)
                factoryImage = Game.Content.Load<Texture2D>("light_free_factory");

            if(type == 1)
                factoryImage = Game.Content.Load<Texture2D>("light_blue_factory");

            if(type == 2)
                factoryImage = Game.Content.Load<Texture2D>("light_red_factory");
        }

        public void LoadImageReinforce()
        {
            factoryImage = Game.Content.Load<Texture2D>("sword");
            //factoryOrigin = new Vector2(factoryImage.Width * 0.5f, factoryImage.Height * 0.5f);
        }

        public Vector2 Center
        {
            get 
            {
                return factoryCenter;
            }

            set
            {
                factoryCenter.X = value.X;
                factoryCenter.Y = value.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(factoryImage, factoryOrigin, Color.White);
            spriteBatch.DrawString(Font1, numTroops.ToString() +"/"+ troopsCap.ToString(), factoryOrigin,Color.Yellow);
            spriteBatch.End();
        }
    }
}
