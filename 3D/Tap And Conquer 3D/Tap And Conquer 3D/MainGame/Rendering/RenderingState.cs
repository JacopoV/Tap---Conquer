using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RenderingState : RenderingExplosion
    {

        public SpriteFont font;
        public SpriteFont fontInfo;
        private Texture2D arrow;
        private Texture2D timeRectTexture;
        private Texture2D backgroundInfo;
        private Texture2D arrowInfo;

        private Texture2D speedText;
        private Texture2D attackText;
        private Texture2D shieldText;
        private Texture2D armageddonText;

        private Texture2D back;
        public Rectangle backButton;

        private Texture2D pause;
        public Rectangle pauseButton;

        private Texture2D posCameraMap;

        private Texture2D inGameMenuText;
        private Rectangle inGameMenu;

        public Rectangle speedBonusRect { get; set; }
        public bool isSpeedBonusActive { get; set; }

        public Rectangle freezeBonusRect { get; set; }
        public bool isFreezeBonusActive { get; set; }

        public Rectangle halfFactoryBonusRect { get; set; }
        public bool isHalfFactoryBonusActive { get; set; }

        public Rectangle armageddonBonusRect { get; set; }
        public bool isArmageddonBonusActive { get; set; }

        public Rectangle mapRect { get; set; }

        public int speedTimer = 0, freezeTimer = 0, halfFactoryTimer = 0, armageddonTimer = 0;

        private float timer = 0;
        private float partial = 0;

        Viewport mapView;
        Viewport defaultViewport;

        public StateGame stateGame;


        public RenderingState(ContentManager Content)
            : base(Content)
        {
            arrow = Content.Load<Texture2D>("Textures/glowingLine");
            fontInfo = Content.Load<SpriteFont>("Fonts/ListOrderFont");
            timeRectTexture = Content.Load<Texture2D>("MenuTexture/timer");
            backgroundInfo = Content.Load<Texture2D>("FactoryTexture/backgroundInfo");
            arrowInfo = Content.Load<Texture2D>("FactoryTexture/arrowInfo");
            inGameMenuText = Content.Load<Texture2D>("GameTexture/hud");
            posCameraMap = Content.Load<Texture2D>("FactoryTexture/cameraPosMap");
            back = Content.Load<Texture2D>("Textures/back");
            pause = Content.Load<Texture2D>("Textures/pause");

            backButton = new Rectangle(0, 0, back.Width, back.Height);
            pauseButton = new Rectangle(back.Width + 5, 0, pause.Width, pause.Height);

            inGameMenu = new Rectangle(
                0, 
                spriteBatch.GraphicsDevice.Viewport.Height - 170, 
                spriteBatch.GraphicsDevice.Viewport.Width, 
                180);

            speedText = Content.Load<Texture2D>("GameTexture/speed");
            attackText = Content.Load<Texture2D>("GameTexture/attack");
            shieldText = Content.Load<Texture2D>("GameTexture/shield");
            armageddonText = Content.Load<Texture2D>("GameTexture/armageddon");

            speedBonusRect = new Rectangle(spriteBatch.GraphicsDevice.Viewport.Width - 82, 5, 80, 80);
            freezeBonusRect = new Rectangle(spriteBatch.GraphicsDevice.Viewport.Width - 82, 90, 80, 80);
            halfFactoryBonusRect = new Rectangle(spriteBatch.GraphicsDevice.Viewport.Width - 82, 175, 80, 80);
            armageddonBonusRect = new Rectangle(spriteBatch.GraphicsDevice.Viewport.Width - 82, 260, 80, 80);

            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;

			defaultViewport = spriteBatch.GraphicsDevice.Viewport;
            mapView = defaultViewport;
            mapView.Width = 200;
            mapView.Height = 150;
            mapView.X = 10;
            mapView.Y = spriteBatch.GraphicsDevice.Viewport.Height-160;
            mapRect = new Rectangle(7, spriteBatch.GraphicsDevice.Viewport.Height - 163, 206, 156);
        }

        

        public void Draw(GameState gameState, GraphicsDevice gd)
        {
            //draw Game
            gd.Viewport = defaultViewport;
            base.DrawExplosions(gameState);

            spriteBatch.Begin();

            foreach (StateGameLayer.Factory i in stateGame.getFactories())
            {
                if (i.Selected == true)
                {
                    int space = 80;
                    int alt = 15;
                    Vector3 screenPoint = spriteBatch.GraphicsDevice.Viewport.Project(
                        i.CurrentPos, Projection, View, World);

                    Vector2 origin = new Vector2(screenPoint.X, screenPoint.Y);
                    Vector2 destination = new Vector2(screenPoint.X + space, screenPoint.Y - space);

                    spriteBatch.Draw(arrowInfo, origin, new Rectangle(0, 0, arrowInfo.Width, arrowInfo.Height), Color.White);

                    spriteBatch.Draw(backgroundInfo, new Rectangle((int)destination.X, (int)destination.Y, 140, 115), new Rectangle(0, 0, backgroundInfo.Width, backgroundInfo.Height), Color.White);
                    spriteBatch.DrawString(fontInfo, "Name: " + i.namePlanet, new Vector2(screenPoint.X + space, screenPoint.Y - space), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Defense: " + i.Defense.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt)), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Shield: " + i.Shields.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt*2)), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Attack: " + i.Damage.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt*3)), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Health: " + i.HitPoints.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt*4)), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Elements: " + i.Elements.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt*5)), Color.DarkBlue);
                    spriteBatch.DrawString(fontInfo, "Area: " + i.Area.ToString(), new Vector2(screenPoint.X + space, screenPoint.Y - (space-alt*6)), Color.DarkBlue);
                    
                }
            }

            spriteBatch.Draw(backgroundInfo, mapRect, Color.White);

            //spriteBatch.Draw(inGameMenuText, inGameMenu, Color.White);

            DrawInfo(gameState.numberOrders(1), gameState.LastTurn, gameState);
            #region Draw the arrows
            if (gameState.arrowPosition.Count == 2)
            {
                if (timer < 2)
                {
                    timer += 0.2f;
                    partial += 0.1f;

                    /*Single arrow from origin factory to destination factory*/
                    Vector3 beginPosition = spriteBatch.GraphicsDevice.Viewport.Project(
                            gameState.arrowPosition[0], Projection, View, World);

                    Vector3 endPosition = spriteBatch.GraphicsDevice.Viewport.Project(
                            gameState.arrowPosition[1], Projection, View, World);

                    Vector2 origin = new Vector2(beginPosition.X, beginPosition.Y);
                    Vector2 destination = new Vector2(endPosition.X, endPosition.Y);


                    Vector2 direction = (destination - origin) * partial;
                    float stretchFactor = (direction).Length() / arrow.Width;
                    double angle = Math.Atan2(direction.Y, direction.X);
                    spriteBatch.Draw(
                        arrow,
                        origin + direction / 2,
                        null,
                        Color.White,
                        (float)angle,
                        new Vector2(arrow.Width, arrow.Height) / 2,
                        new Vector2(stretchFactor, 1),
                        SpriteEffects.None,
                        0);
                }
                else
                {
                    timer = 0;
                    partial = 0;
                    gameState.arrowPosition.Clear();
                }

            }

            if (gameState.arrowPosition.Count == 3)
            {
                if (timer < 2)
                {
                    timer += 0.2f;
                    partial += 0.1f;

                    /*Double arrow*/
                    Vector3 beginPosition = spriteBatch.GraphicsDevice.Viewport.Project(
                        gameState.arrowPosition[0], Projection, View, World);

                    Vector3 beginPosition2 = spriteBatch.GraphicsDevice.Viewport.Project(
                        gameState.arrowPosition[1], Projection, View, World);

                    Vector3 endPosition = spriteBatch.GraphicsDevice.Viewport.Project(
                            gameState.arrowPosition[2], Projection, View, World);

                    Vector2 origin = new Vector2(beginPosition.X, beginPosition.Y);
                    Vector2 origin2 = new Vector2(beginPosition2.X, beginPosition2.Y);
                    Vector2 destination = new Vector2(endPosition.X, endPosition.Y);


                    /*First arrow*/
                    Vector2 direction = (destination - origin) * partial;
                    float stretchFactor = (direction).Length() / arrow.Width;
                    double angle = Math.Atan2(direction.Y, direction.X);
                    spriteBatch.Draw(
                        arrow,
                        origin + direction / 2,
                        null,
                        Color.White,
                        (float)angle,
                        new Vector2(arrow.Width, arrow.Height) / 2,
                        new Vector2(stretchFactor, 1),
                        SpriteEffects.None,
                        0);

                    /*Second arrow*/
                    Vector2 direction2 = (destination-origin2) * partial;
                    float stretchFactor2 = (direction2).Length() / arrow.Width;
                    double angle2 = Math.Atan2(direction2.Y, direction2.X);
                    spriteBatch.Draw(
                        arrow,
                        origin2 + direction2 / 2,
                        null,
                        Color.White,
                        (float)angle2,
                        new Vector2(arrow.Width, arrow.Height) / 2,
                        new Vector2(stretchFactor2, 1),
                        SpriteEffects.None,
                        0);
                }
                else
                {
                    timer = 0;
                    partial = 0;
                    gameState.arrowPosition.Clear();
                }
 
            }
            #endregion 

            drawBonus();
            spriteBatch.Draw(back, backButton, Color.White);
            spriteBatch.Draw(pause, pauseButton, Color.White);

            spriteBatch.End();

            gd.Viewport = mapView;
            base.DrawTroop(gameState, true);
            spriteBatch.Begin();
            spriteBatch.Draw(posCameraMap, new Rectangle((int)(Camera.X / 50)+100,(int)(Camera.Z / 46.6)+75,20,20), Color.White);
            spriteBatch.End();

            gd.Viewport = defaultViewport;

        }

        public void DrawInfo(int numberOrders, float time, GameState gs)
        {
            int seconds = (int)time;
            int decimals = (int)((time - seconds) * 10);
            spriteBatch.DrawString(
                spriteFont, 
                "Order List:" + numberOrders.ToString() + "/3", 
                new Vector2(
                    spriteBatch.GraphicsDevice.Viewport.Width - 200, 
                    spriteBatch.GraphicsDevice.Viewport.Height - 70),
                numberOrders == 3 ? Color.Red : numberOrders == 2 ? Color.Orange : Color.White);
            
            /*Leave this code for debug */
            //spriteBatch.DrawString(
            //    fontInfo, 
            //    "Turn Time:" + seconds + "." + decimals + "s/5s",
            //    new Vector2(10, spriteBatch.GraphicsDevice.Viewport.Height - 50), 
            //    Color.WhiteSmoke);

            float stepDecimals =(350 / (gs.turn * 10));

            spriteBatch.Draw(timeRectTexture, new Rectangle(spriteBatch.GraphicsDevice.Viewport.Width / 2, spriteBatch.GraphicsDevice.Viewport.Height - 40, (int)(350 - (seconds * stepDecimals*10 + decimals * stepDecimals)), 20), Color.White);
        }

        public void drawBonus()
        {
            spriteBatch.Draw(speedText, speedBonusRect, isSpeedBonusActive ? Color.White : Color.DarkGray);
            spriteBatch.Draw(timeRectTexture, new Rectangle(speedBonusRect.Left, speedBonusRect.Bottom, speedTimer*8, 4), Color.White);
            
            spriteBatch.Draw(attackText, freezeBonusRect, isFreezeBonusActive ? Color.White : Color.DarkGray);
            spriteBatch.Draw(timeRectTexture, new Rectangle(freezeBonusRect.Left, freezeBonusRect.Bottom, freezeTimer*8, 4), Color.White);

            spriteBatch.Draw(shieldText, halfFactoryBonusRect, isHalfFactoryBonusActive ? Color.White : Color.DarkGray);
            spriteBatch.Draw(timeRectTexture, new Rectangle(halfFactoryBonusRect.Left, halfFactoryBonusRect.Bottom, halfFactoryTimer*8, 4), Color.White);

            spriteBatch.Draw(armageddonText, armageddonBonusRect, isArmageddonBonusActive ? Color.White : Color.DarkGray);
            spriteBatch.Draw(timeRectTexture, new Rectangle(armageddonBonusRect.Left, armageddonBonusRect.Bottom, armageddonTimer*8, 4), Color.White);
        }


    }

}
