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

namespace TapAndConquer
{
    public class Manager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private IA computer;
        private SpriteBatch spriteBatch;
        private GestureSample touchGesture;
        private List<Factory> factories = new List<Factory>();
        private List<Troop> troops = new List<Troop>();
        private List<Order> player1_orders = new List<Order>();
        private List<Order> player2_orders = new List<Order>();

        private Texture2D terrain, arrow;
        private bool touchBegin = false;
        private bool needReinforcement = false;
        private float touchDelta = 0f, touchPartial = 0f;
        private Vector2 arrowOrigin, arrowDestination, arrowReinforcement;
        private Factory factoryOrigin, factoryDestination, factoryReinforcement;

        private Factory factoryReinforceSelected = null;

        private static int capitalFactoryCap = 20;
        private static int neutralFactoryMinCap = 2;
        private static int neutralFactoryMaxCap = 15;
        private static int factoryMinNum = 8;
        private static int factoryMaxNum = 15;

        private float turn = 5;
        private float lastTurn = 0;

        public event Action win;
        public event Action loose;

        public Manager(Game game): base(game)
        {
            computer = new IA(this);

            Random generation = new System.Random();

            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.DragComplete;

            List<Vector2> coordFactory = new List<Vector2>();

            int numFactory = generation.Next(8, 15);

            int widthFactory = Game.GraphicsDevice.Viewport.Width / 7;
            int heightFactory = Game.GraphicsDevice.Viewport.Height / 3;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                    coordFactory.Add(new Vector2(widthFactory * i, heightFactory * j));
            }

            for (int i = 0; i < numFactory; i++)
            {
                int posFactory = generation.Next(0, coordFactory.Count);

                if (i == 0)
                {
                    factories.Add(new Factory(game, 1, coordFactory[posFactory], capitalFactoryCap));
                }
                else if (i == numFactory - 1)
                {
                    factories.Add(new Factory(game, 2, coordFactory[posFactory], capitalFactoryCap));
                }
                else
                {
                    factories.Add(new Factory(game, 0, coordFactory[posFactory], generation.Next(neutralFactoryMinCap, neutralFactoryMaxCap)));
                }

                coordFactory.Remove(coordFactory[posFactory]);
            }

            computer.loadNeutralFactories();
        }

        public override void Initialize()
        {
            spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < factories.Count; i++)
                factories[i].loadImages();

            terrain = Game.Content.Load<Texture2D>("terrain");
            arrow = Game.Content.Load<Texture2D>("arrow");
        }

        public override void Update(GameTime gameTime)
        {
            var deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastTurn += deltaT;

            if (lastTurn >= turn)
            {
                lastTurn = 0;
                bool done = false;
                for (int i = 0; i < player1_orders.Count && !done; ++i)
                {
                    List<Troop> newTroop = player1_orders[0].execute(1);
                    player1_orders.RemoveAt(0);
                    --i;
                    if (newTroop != null)
                    {
                        for (int j = 0; j < newTroop.Count; ++j)
                        {
                            troops.Add(newTroop[j]);
                            troops[troops.Count - 1].LoadImages();
                        }
                        newTroop.Clear();
                        done = true;
                    }
                }
                done = false;
                for (int i = 0; i < player2_orders.Count && !done; ++i)
                {
                    List<Troop> newTroop = player2_orders[0].execute(2);
                    player2_orders.RemoveAt(0);
                    --i;
                    if (newTroop != null)
                    {
                        for (int j = 0; j < newTroop.Count; ++j)
                        {
                            troops.Add(newTroop[j]);
                            troops[troops.Count - 1].LoadImages();
                        }
                        newTroop.Clear();
                        done = true;
                    }
                }
            }

            if (player2_orders.Count < 3)
                computer.useIA(deltaT);

            //manage touch events
#if WINDOWS_PHONE
            ManageTouch(gameTime);
#endif
#if WINDOWS      
               ManageClick(gameTime);
#endif

            bool isEndGame = true;
            int currentWinnerType = factories[0].getType();

            for (int i = 0; i < factories.Count && isEndGame; i++)
            {
                if (factories[i].getType() != 0)
                    if (currentWinnerType != factories[i].getType())

                        isEndGame = false;
            }

            if (isEndGame)
            {
                if (currentWinnerType == 1)
                    win();
                else
                    loose();
            }

            for (int i = 0; i < factories.Count; ++i)
            {
                factories[i].update(deltaT);
            }

            for (int i = 0; i < troops.Count; ++i)
            {
                if (troops[i].update(deltaT))
                {
                    Vector2 target = troops[i].getTarget();
                    int nFactory = this.search(target);
                    if (nFactory != -1) factories[nFactory].manageArrival(troops[i]);
                    troops.RemoveAt(i);
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            spriteBatch.Draw(terrain, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            spriteBatch.DrawString(Game.Content.Load<SpriteFont>("ListOrderFont"), "Order List:" + numberOrders(1).ToString() + "/3", new Vector2(GraphicsDevice.Viewport.Width - 250, GraphicsDevice.Viewport.Height - 50), Color.WhiteSmoke);

            spriteBatch.End();

            for (int i = 0; i < factories.Count; i++)
            {
                factories[i].Draw(spriteBatch);
            }

            for (int i = 0; i < troops.Count; i++)
            {
                troops[i].Draw(spriteBatch);
            }

            // draw arrow for touch events
            spriteBatch.Begin();
            if (touchBegin)
            {
                if (matchPlayerFactory())
                {
                    Vector2 originMinusDestination = arrowDestination - arrowOrigin;
                    float stretchFactor = (originMinusDestination).Length() / arrow.Width;
                    double angle = Math.Atan2(originMinusDestination.Y, originMinusDestination.X);
                    spriteBatch.Draw(arrow, arrowOrigin + originMinusDestination / 2, null, Color.White, (float)angle, new Vector2(arrow.Width, arrow.Height) / 2, new Vector2(stretchFactor, 1), SpriteEffects.None, 0);

                }
            }
            spriteBatch.End();
        }

        private int search(Vector2 coordinates)
        {
            for (int i = 0; i < factories.Count; ++i)
            {
                if (factories[i].getCenter() == coordinates)
                    return i;
            }
            return -1;
        }

        public List<Factory> getFactories()
        {
            return factories;
        }

        public bool addOrder(List<Factory> newOrder, int player)
        {
            if (player == 1 && player1_orders.Count < 3)
            {
                player1_orders.Add(new Order(base.Game, newOrder));
                if (player1_orders.Count < 3)
                    return true;
            }
            else if (player == 2 && player2_orders.Count < 3)
            {
                player2_orders.Add(new Order(base.Game, newOrder));
                if (player2_orders.Count < 3)
                    return true;
            }
            return false;
        }

#if WINDOWS_PHONE
        private void ManageTouch(GameTime time)
        {
            if (numberOrders(1) < 3)
            {
                while (TouchPanel.IsGestureAvailable)
                {
                    touchGesture = TouchPanel.ReadGesture();
                    if (touchGesture.GestureType == GestureType.FreeDrag)
                    {
                        if (!touchBegin)
                        {
                            // save the state when drag starts
                            arrowOrigin = new Vector2(touchGesture.Position.X, touchGesture.Position.Y);
                            touchDelta = (float)time.ElapsedGameTime.TotalSeconds;
                            touchBegin = true;
                            factoryDestination = null;
                            factoryOrigin = null;
                            factoryReinforcement = null;
                            needReinforcement = false;
                        }
                        // update the current position beause may be the final destination
                        arrowDestination = new Vector2(touchGesture.Position.X, touchGesture.Position.Y);
                        // GLOW FOR EVERY FACTORY
                        foreach (Factory i in factories)
                        {
                            if (i != factoryReinforceSelected && factoryOrigin != null)
                            {
                                if (factoryOrigin.getType() == 1)
                                {
                                    Vector2 factoryPos = i.Center;
                                    float rad = i.getRadius();
                                    if (arrowDestination.X <= factoryPos.X + 30 &&
                                         arrowDestination.X >= factoryPos.X - 30 &&
                                           arrowDestination.Y <= factoryPos.Y + 30 &&
                                             arrowDestination.Y >= factoryPos.Y - 30)
                                    {
                                        touchPartial += (float)time.ElapsedGameTime.TotalSeconds;
                                        //the reinforcement factory
                                        i.loadImageWhenArrowOver();
                                    }
                                    else
                                    {
                                        i.loadImages();
                                    }
                                }
                            }
                        }

                        if (touchPartial - touchDelta > 1 && !needReinforcement)
                        {
                            //need reinforcements 
                            arrowReinforcement = new Vector2(touchGesture.Position.X, touchGesture.Position.Y);
                            needReinforcement = true;
                            touchDelta = 0;
                            touchPartial = 0;

                            // GLOW FOR REINFORCEMENT FACTORY
                            foreach (Factory i in factories)
                            {
                                Vector2 factoryPos = i.Center;
                                float rad = i.getRadius();
                                if (arrowReinforcement.X <= factoryPos.X + 30 &&
                                     arrowReinforcement.X >= factoryPos.X - 30 &&
                                       arrowReinforcement.Y <= factoryPos.Y + 30 &&
                                         arrowReinforcement.Y >= factoryPos.Y - 30)
                                {
                                    //the reinforcement factory
                                    if (i.getType() == 1)
                                    {
                                        if (i != factoryOrigin)
                                        {
                                            i.LoadImageReinforce();
                                            factoryReinforceSelected = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (touchGesture.GestureType == GestureType.DragComplete)
                    {
                        //flag to stop drawing and launch order
                        touchBegin = false;
                        touchDelta = 0;
                        touchPartial = 0;
                        factoryReinforceSelected = null;
                        //check if the destination is a factory --> launch order
                        manageDestinationFactory();

                    }
                }
            }
        }
#endif
#if WINDOWS
        private void ManageClick(GameTime time)
        {
            if (numberOrders(1) < 3)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!touchBegin)
                    {
                        // save the state when drag starts
                        arrowOrigin = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                        touchDelta = (float)time.ElapsedGameTime.TotalSeconds;
                        touchBegin = true;
                        factoryDestination = null;
                        factoryOrigin = null;
                        factoryReinforcement = null;
                        needReinforcement = false;

                    }

                    // update the current position beause may be the final destination
                    arrowDestination = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                    // GLOW FOR EVERY FACTORY
                    foreach (Factory i in factories)
                    {
                        if (i != factoryReinforceSelected && factoryOrigin != null)
                        {
                            if (factoryOrigin.getType() == 1)
                            {
                                Vector2 factoryPos = i.Center;
                                float rad = i.getRadius();
                                if (arrowDestination.X <= factoryPos.X + 30 &&
                                     arrowDestination.X >= factoryPos.X - 30 &&
                                       arrowDestination.Y <= factoryPos.Y + 30 &&
                                         arrowDestination.Y >= factoryPos.Y - 30)
                                {
                                    //the reinforcement factory
                                    i.loadImageWhenArrowOver();
                                    touchPartial += (float)time.ElapsedGameTime.TotalSeconds;
                                }
                                else
                                {
                                    i.loadImages();
                                }
                            }
                        }
                    }

                    if (touchPartial - touchDelta > 1 && !needReinforcement)
                    {
                        //need reinforcements 
                        arrowReinforcement = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                        needReinforcement = true;
                        touchDelta = 0f;
                        touchPartial = 0f;

                        // GLOW FOR REINFORCEMENT FACTORY
                        foreach (Factory i in factories)
                        {
                            Vector2 factoryPos = i.Center;
                            float rad = i.getRadius();
                            if (arrowReinforcement.X <= factoryPos.X + 30 &&
                                 arrowReinforcement.X >= factoryPos.X - 30 &&
                                   arrowReinforcement.Y <= factoryPos.Y + 30 &&
                                     arrowReinforcement.Y >= factoryPos.Y - 30)
                            {
                                //the reinforcement factory
                                if (i.getType() == 1)
                                {
                                    if (i != factoryOrigin)
                                    {
                                        i.LoadImageReinforce();
                                        factoryReinforceSelected = i;
                                    }
                                }
                            }
                        }
                    }
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    //flag to stop drawing and launch order
                    touchBegin = false;

                    //check if the destination is a factory --> launch order
                    manageDestinationFactory();

                    factoryReinforceSelected = null;

                    touchDelta = 0f;
                    touchPartial = 0f;

                }
            }
        }
        #endif

        private bool matchPlayerFactory()
        {

            foreach (Factory i in factories)
            {
                Vector2 factoryPos = i.Center;
                float rad = i.getRadius();
                if (rad > 0)
                {
                    if (arrowOrigin.X <= factoryPos.X + 30 &&
                         arrowOrigin.X >= factoryPos.X - 30 &&
                           arrowOrigin.Y <= factoryPos.Y + 30 &&
                             arrowOrigin.Y >= factoryPos.Y - 30)
                    {
                        if (i.getType() == 1)
                        {
                            factoryOrigin = i;
                            return true;
                        }
                    }
                }
            }
            factoryOrigin = null;
            return false;
        }

        private bool orderFull(int player)
        {
            if (player == 1 && player1_orders.Count > 2)
                return true;
            if (player == 2 && player2_orders.Count > 2)
                return true;
            return false;
        }

        private int numberOrders(int player)
        {
            if (player == 1)
                return player1_orders.Count;
            if (player == 2)
                return player2_orders.Count;
            return -1;
        }

        private void manageDestinationFactory()
        {

            if (needReinforcement)
            {

                foreach (Factory i in factories)
                {
                    Vector2 factoryPos = i.Center;
                    float rad = i.getRadius();
                    if (arrowReinforcement.X <= factoryPos.X + 30 &&
                         arrowReinforcement.X >= factoryPos.X - 30 &&
                           arrowReinforcement.Y <= factoryPos.Y + 30 &&
                             arrowReinforcement.Y >= factoryPos.Y - 30)
                    {
                        //the reinforcement factory
                        factoryReinforcement = i;
                        factoryReinforcement.loadImages();

                    }
                }

            }

            if (factoryOrigin != null)
            {
                foreach (Factory i in factories)
                {
                    Vector2 factoryPos = i.Center;
                    float rad = i.getRadius();
                    if (arrowDestination.X <= factoryPos.X + 30 &&
                         arrowDestination.X >= factoryPos.X - 30 &&
                           arrowDestination.Y <= factoryPos.Y + 30 &&
                             arrowDestination.Y >= factoryPos.Y - 30)
                    {
                        //the target is a factory
                        factoryDestination = i;


                    }
                }

                if (factoryDestination != null)
                {
                    List<Factory> newOrder = new List<Factory>();
                    newOrder.Add(factoryOrigin);
                    if (needReinforcement)
                    {
                        if (factoryReinforcement != null)
                        {
                            newOrder.Add(factoryReinforcement);
                            // factoryReinforcement.loadImageSelected();   // ALFY    
                            needReinforcement = false;
                        }

                    }
                    newOrder.Add(factoryDestination);
                    addOrder(newOrder, 1);



                    ///////
                    ////  DEBUG MODE  /////
                    ///////
                    //move the tanks
                    if (factoryOrigin.getSendableTroops(false) > 0)
                    {
                        //troops.Add(new Troop(base.Game, 1, factoryOrigin.Center, factoryOrigin.getSendableTroops(true), factoryDestination.Center, factoryDestination));
                        //troops[troops.Count - 1].LoadImages();

                        if (needReinforcement)
                        {

                            if (factoryReinforcement != null)
                            {
                                //troops.Add(new Troop(base.Game, 1, factoryReinforcement.Center, factoryReinforcement.getSendableTroops(true), factoryDestination.Center, factoryDestination));
                                //troops[troops.Count - 1].LoadImages();
                                needReinforcement = false;
                            }

                        }

                    }

                    //orderd lanuched
                    factoryDestination = null;
                    factoryOrigin = null;
                    factoryReinforcement = null;
                    arrowReinforcement = new Vector2(0, 0);

                    //reset factory images

                    foreach (Factory i in factories)
                    {
                        i.loadImages();

                    }



                }
            }
        }
    }

}
