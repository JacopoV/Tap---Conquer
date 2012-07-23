using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace TapAndConquer.GameObjects
{
    public class Troop : GameObject
    {
        private bool fighting;
        private Factory myTarget;

        private const int width = 20;
        private const int height = 40;

        private Troop myFellow = null;
        private bool myFellowArrived = false;

        public Troop(int owner, Vector2 position, int elements, Factory targetFactory)
            :base(position, owner, elements)
        {
            Rotation = (float)Math.Atan2(targetFactory.Center.Y - position.Y, targetFactory.Center.X - position.X);
            myTarget = targetFactory;
            Speed = 30.0f;
            Velocity = new Vector2( (float)Math.Cos(Rotation), (float)Math.Sin(Rotation) );
            fighting = false;
            TimeLimit = 4;
            StartTime = 0;

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

        //public override void loadImage()
        //{
        //    if(ObjectType==1)
        //       Image = Game.Content.Load<Texture2D>("TroopTexture/blue_tank");
        //    if (ObjectType == 2)
        //        Image = Game.Content.Load<Texture2D>("TroopTexture/red_tank");
        //    Radius = Image.Height;
        //    Center = new Vector2(CurrentPos.X + Image.Width * 0.5f, CurrentPos.Y + Image.Height * 0.5f);
        //}

        public void battle() 
        {
            myTarget.attack();
            fighting = true;
            //loadImageBattle();
        }

        //public void loadImageBattle()
        //{
        //    Image = Game.Content.Load<Texture2D>("TroopTexture/war");
        //    Center = new Vector2(CurrentPos.X + Image.Width * 0.5f, CurrentPos.Y + Image.Height * 0.5f);
        //}

        public bool Fighting
        {
            set { fighting = value; }

            get { return fighting; }
        }

        public bool isArrivedAtTarget()
        {
            return (Vector2.Distance(CurrentPos, myTarget.Center) < Radius);
        }

        public Factory Target
        {
            set { myTarget = value; }
            get { return myTarget; }
        }

        public Troop Fellow
        {
            set { myFellow = value; }
            get { return myFellow; }
        }

        public bool FellowArrived
        {
            set { myFellowArrived = value; }
            get { return myFellowArrived; }
        }

        //public bool update(float deltaT)
        //{
            //if (isFighting)
            //{
            //    StartTime += deltaT;
            //    if (StartTime >= TimeLimit)
            //        return true;
            //}
            //else
            //{
            //    if (this.isArrivedAtTarget())
            //    {
            //        bool finish = true;
            //        if (myFellow != null)
            //        {
            //            finish = false;
            //            if (myTarget.ObjectType == ObjectType || myTarget.ObjectType == 0)
            //            {
            //                finish = true;
            //                myFellow.setFellow(null);
            //            }
            //            else
            //            {
            //                if (!myFellowArrived)
            //                    myFellow.setFellowArrived(true);
            //                else
            //                {
            //                    if (Elements > 0)
            //                    {
            //                        myFellow.setFellowArrived(true);
            //                        Elements += myFellow.Elements;
            //                        myFellow.Elements = 0;
            //                    }
            //                    finish = true;
            //                }
            //            }
            //        }
            //        if (finish)
            //        {
            //            if (myTarget.ObjectType != ObjectType && myTarget.ObjectType != 0)
            //                this.battle();
            //            else
            //                return true;
            //        }
            //    }
            //    else
            //    {
            //        CurrentPos = new Vector2(CurrentPos.X + Velocity.X * Speed * deltaT, CurrentPos.Y + Velocity.Y * Speed * deltaT);
            //        Center = new Vector2(CurrentPos.X + Image.Width * 0.5f, CurrentPos.Y + Image.Height * 0.5f);
            //    }
            //}
            //return false;
        //}

        //public void Draw(SpriteBatch spriteBatch)
        //{
        //    Vector2 troopOrigin = new Vector2(Image.Width * 0.5f, Image.Height * 0.5f);
        //    spriteBatch.Begin();
        //    spriteBatch.Draw(Image, CurrentPos, null, Color.White, Rotation, troopOrigin, 1.0f, SpriteEffects.None, 1.0f);
        //    spriteBatch.DrawString(Font, Elements.ToString(), CurrentPos, Color.Yellow);
        //    spriteBatch.End();
        //}
    };
}
