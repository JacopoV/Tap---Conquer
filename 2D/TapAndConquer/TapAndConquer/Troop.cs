using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace TapAndConquer
{
    public class Troop : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteFont font;
        private Texture2D troopImg;
        private int owner;
        private int elements;
        private int radius;
        private float troopRotation;
        private Vector2 troopOrigin;
        private Vector2 currentPos;
        private Vector2 target;
        private float speed;
        private Vector2 velocity;
        private bool fighting;
        private float battleTime;
        private float startBattleTime;
        private Factory myTarget;

        private Troop myFellow = null;
        private bool myFellowArrived = false;

        public Troop(Game game, int owner, Vector2 origin, int elements, Vector2 targetFactoryCoordinates, Factory targetFactory)//rivedi i parametri del costruttore
            :base(game)
        {
            float angle = (float)Math.Atan2(targetFactoryCoordinates.Y - origin.Y, targetFactoryCoordinates.X - origin.X);
            this.owner = owner;
            this.elements = elements;
            this.myTarget = targetFactory;
            troopRotation = angle;
            currentPos = new Vector2(origin.X, origin.Y);
            target = new Vector2(targetFactoryCoordinates.X, targetFactoryCoordinates.Y);
            speed = 30.0f;
            velocity.X = (float)Math.Cos(angle);
            velocity.Y = (float)Math.Sin(angle);
            fighting = false;
            battleTime = 4;
            startBattleTime = 0;
        }

        public void LoadImages()
        {
            if(owner==1)
                troopImg = Game.Content.Load<Texture2D>("blue_tank");

            if(owner==2)
                troopImg = Game.Content.Load<Texture2D>("red_tank");

            radius = troopImg.Height;
            troopOrigin = new Vector2(troopImg.Width * 0.5f, troopImg.Height * 0.5f);
            font = Game.Content.Load<SpriteFont>("FactoryFont");
        }

        public void battle() 
        {
            myTarget.attack();
            fighting = true;
            loadImageBattle();
        }

        public void loadImageBattle()
        {
            troopImg = Game.Content.Load<Texture2D>("war");
            troopOrigin = new Vector2(troopImg.Width * 0.5f, troopImg.Height * 0.5f);
        }

        public bool isFighting
        {
            set
            {
                fighting = value;
            }

            get 
            {
                return fighting;
            }
        }

        public Vector2 Position
        {
            set
            {
                currentPos.X = value.X;
                currentPos.Y = value.Y;
            }

            get
            {
                return currentPos;
            }
        }

        public float Rotation
        {
            set
            {
                troopRotation = value;
            }

            get
            {
                return troopRotation;
            }
        }

        float getSpeed()
        {
            return speed;
        }

        public void setSpeed(float x)
        {
            speed = x;
        }

        public int getOwner()
        {
            return owner;
        }

        public void setOwner(int owner)
        {
            this.owner = owner;
        }

        public int getElements()
        {
            return elements;
        }

        public Vector2 getTarget()
        {
            return target;
        }

        public void setElements(int elements)
        {
            this.elements = elements;
        }

        public bool isArrivedAtTarget()
        {
            return (Vector2.Distance(currentPos, target) < radius);
        }

        public void setFellow(Troop fellow) 
        {
            myFellow = fellow;
        }

        public void setFellowArrived(bool arrived)
        {
            myFellowArrived = arrived;
        }

        public bool update(float deltaT)
        {
            if (isFighting)
            {
                startBattleTime += deltaT;
                if (startBattleTime >= battleTime)
                    return true;
            }
            else
            {
                if (this.isArrivedAtTarget())
                {
                    bool finish = true;
                    if (myFellow != null)
                    {
                        finish = false;
                        if (myTarget.getType() == owner || myTarget.getType() == 0)
                        {
                            finish = true;
                            myFellow.setFellow(null);
                        }
                        else
                        {
                            if (!myFellowArrived)
                                myFellow.setFellowArrived(true);
                            else
                            {
                                if (elements > 0)
                                {
                                    myFellow.setFellowArrived(true);
                                    elements += myFellow.getElements();
                                    myFellow.setElements(0);
                                }
                                finish = true;
                            }
                        }
                    }
                    if (finish) 
                    {
                        if (myTarget.getType() != owner && myTarget.getType() != 0)
                            this.battle();
                        else
                            return true;
                    }
                }
                else
                {
                    currentPos.X += velocity.X * speed * deltaT;
                    currentPos.Y += velocity.Y * speed * deltaT;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(troopImg, currentPos, null, Color.White, troopRotation, troopOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, elements.ToString(), currentPos, Color.Yellow);
            spriteBatch.End();
        }
    };
}
