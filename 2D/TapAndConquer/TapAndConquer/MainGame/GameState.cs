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

using TapAndConquer.GameObjects;


namespace TapAndConquer
{
  using Difficulty = GameDifficulty;
  using System.Threading;
  using Microsoft.Devices;

  public enum WinLoseState
  {
    Win,
    Lose,
    None
  }

  public class GameState : IAState
  {
  
    public Difficulty difficulty;
    public double totalGameTime;
    public int score;

    //OWN VARIABLE
    private List<Order> player1_orders = new List<Order>();
    private List<Order> player2_orders = new List<Order>();

    private bool touchBegin = false;
    private bool needReinforcement = false;
    private float touchDelta = 0f, touchPartial = 0f;
    private Vector2 arrowOrigin, arrowDestination, arrowReinforcement;
    private Factory factoryOrigin, factoryDestination, factoryReinforcement;

    private Factory factoryReinforceSelected = null;

    private float turn = 5;
    private float lastTurn = 0;

    //COSTRUCTOR
    public GameState() {}
    public GameState(Difficulty difficulty)
    {
        this.difficulty = difficulty;
        totalGameTime = 0.0;
        score = 0;
        setIAGameState(this);
        loadNeutralFactories();
    }

    //UPDATE GAME
    public void UpdateTime(GameTime gameTime, RenderingState renderingState)
    {
      var deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

      totalGameTime += gameTime.ElapsedGameTime.TotalSeconds;

      lastTurn += deltaT;

      if (lastTurn >= turn)
      {
          lastTurn = 0;
          bool done = false;
          for (int i = 0; i < player1_orders.Count && !done; ++i)
          {
              int create = 0;
              if (player1_orders[i].Origin.Count > 0 && player1_orders[i].Origin[0].ObjectType == 1 && player1_orders[i].Origin[0].getSendableTroops(false) > 0)
              {
                  addTroop(player1_orders[i].Origin[0].ObjectType, player1_orders[i].Origin[0].Center, player1_orders[i].Origin[0].getSendableTroops(true), player1_orders[i].Target[0]);
                  create++;
              }
              if (player1_orders[i].Origin.Count > 1 && player1_orders[i].Origin[1].ObjectType == 1 && player1_orders[i].Origin[1].getSendableTroops(false) > 0)
              {
                  addTroop(player1_orders[i].Origin[1].ObjectType, player1_orders[i].Origin[1].Center, player1_orders[i].Origin[1].getSendableTroops(true), player1_orders[i].Target[0]);
                  if (create > 0)
                      setFellow();
                  create++;
              }
              player1_orders.RemoveAt(0);
              i--;
              if (create > 0)
                  done = true;
          }
          done = false;

          for (int i = 0; i < player2_orders.Count && !done; ++i)
          {
              int create = 0;
              if (player2_orders[i].Origin.Count > 0 && player2_orders[i].Origin[0].ObjectType == 2 && player2_orders[i].Origin[0].getSendableTroops(false) > 0)
              {
                  addTroop(player2_orders[i].Origin[0].ObjectType, player2_orders[i].Origin[0].Center, player2_orders[i].Origin[0].getSendableTroops(true), player2_orders[i].Target[0]);
                  create++;
              }
              if (player2_orders[i].Origin.Count > 1 && player2_orders[i].Origin[1].ObjectType == 2 && player2_orders[i].Origin[1].getSendableTroops(false) > 0)
              {
                  addTroop(player2_orders[i].Origin[1].ObjectType, player2_orders[i].Origin[1].Center, player2_orders[i].Origin[1].getSendableTroops(true), player2_orders[i].Target[0]);
                  if (create > 0)
                      setFellow();
                  create++;
              }
              player2_orders.RemoveAt(0);
              i--;
              if (create > 0)
                  done = true;
          }
      }

      //if (player2_orders.Count < 3)
      //    computer.useIA(deltaT);

      //manage touch events
#if WINDOWS_PHONE
      //ManageTouch(gameTime);
#endif
#if WINDOWS      
        //ManageClick(gameTime);
#endif

      base.update(deltaT);
    }

    //ARROW ORIGIN SET/GET
    public Vector2 ArrowOrigin
    {
        set { arrowOrigin = value; }

        get { return arrowOrigin; }
    }

    //ARROW DESTINATION SET/GET
    public Vector2 ArrowDestination
    {
        set { arrowDestination = value; }

        get { return arrowDestination; }
    }

    //LASTTURN SET/GET
    public float LastTurn
    {
        set { lastTurn = value; }

        get { return lastTurn; }
    }

    //TOUCHBEGIN SET/GET
    public bool TouchBegin
    {
        set { touchBegin = value; }

        get { return touchBegin; }
    }

    // RETURN TRUE IF ORDER LIST IS FULL
    private bool orderFull(int player)
    {
        if (player == 1 && player1_orders.Count > 2)
            return true;
        if (player == 2 && player2_orders.Count > 2)
            return true;
        return false;
    }

    //RETURN COUNT OF ORDER
    public int numberOrders(int player)
    {
        if (player == 1)
            return player1_orders.Count;
        if (player == 2)
            return player2_orders.Count;
        return -1;
    }

    private bool matchPlayerFactory()
    {
        foreach (Factory i in factories)
        {
            float distance = Vector2.Distance(i.Center, new Vector2(arrowOrigin.X, arrowOrigin.Y));
            if (distance < i.Radius && i.ObjectType == 1)
            {
                factoryOrigin = i;
                return true;
            }
        }
        factoryOrigin = null;
        return false;
    }

    //ADD NEW ORDER
    public bool addOrder(List<Factory> newOrder, int player)
    {
        if (player == 1 && player1_orders.Count < 3)
        {
            player1_orders.Add(new Order(newOrder));
            if (player1_orders.Count < 3)
                return true;
        }
        else if (player == 2 && player2_orders.Count < 3)
        {
            player2_orders.Add(new Order(newOrder));
            if (player2_orders.Count < 3)
                return true;
        }
        return false;
    }


    private void manageDestinationFactory()
    {

        if (needReinforcement)
        {

            foreach (Factory i in factories)
            {
                float distance = Vector2.Distance(i.Center, new Vector2(arrowReinforcement.X, arrowReinforcement.Y));
                if (distance < i.Radius)
                {
                    //the reinforcement factory
                    factoryReinforcement = i;
                    factoryReinforcement.Reinforced = true;
                }
            }

        }

        if (factoryOrigin != null)
        {
            foreach (Factory i in factories)
            {
                float distance = Vector2.Distance(i.Center, new Vector2(arrowDestination.X, arrowDestination.Y));
                if (distance < i.Radius)
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
                        needReinforcement = false;
                        factoryReinforcement.Selected = false;
                        factoryReinforcement.Reinforced = false;
                    }

                }
                factoryDestination.Selected = false;


                newOrder.Add(factoryDestination);
                addOrder(newOrder, 1);

                /*
                 * Order launched, reset info
                 */
                factoryDestination = null;
                factoryOrigin = null;
                factoryReinforcement = null;
                arrowReinforcement = new Vector2(0, 0);

                //reset factory images

            }
        }
    }


    //UPDATE INPUT
    public void UpdateInput(RenderingState renderingState, GameTime time)
    {
        while (TouchPanel.IsGestureAvailable)
        {
            var touchGesture = TouchPanel.ReadGesture();
            switch (touchGesture.GestureType)
            {
                case GestureType.FreeDrag:
                    /*If orders availables do something*/
                    if (numberOrders(1) < 3)
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

                        if (!matchPlayerFactory())
                            touchBegin = false;
                        // update the current position beause may be the final destination
                        arrowDestination = new Vector2(touchGesture.Position.X, touchGesture.Position.Y);
                        // GLOW FOR EVERY FACTORY
                        foreach (Factory i in factories)
                        {
                            if (i != factoryReinforceSelected && factoryOrigin != null)
                            {
                                if (factoryOrigin.ObjectType == 1)
                                {
                                    float distance = Vector2.Distance(i.Center, new Vector2(arrowDestination.X, arrowDestination.Y));
                                    if (distance < i.Radius)
                                    {
                                        touchPartial += (float)time.ElapsedGameTime.TotalSeconds;

                                        /*
                                         * Select the factory
                                         */
                                        i.Selected = true;

                                    }
                                    else
                                    {
                                        i.Selected = false;
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
                                float distance = Vector2.Distance(i.Center, new Vector2(arrowReinforcement.X, arrowReinforcement.Y));
                                if (distance < i.Radius)
                                {
                                    //the reinforcement factory
                                    if (i.ObjectType == 1)
                                    {
                                        if (i != factoryOrigin)
                                        {
                                            i.Reinforced = true;
                                            factoryReinforceSelected = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case GestureType.DragComplete:

                    /*
                     * Flag to stop drawing and launch order
                     */
                    touchBegin = false;
                    touchDelta = 0.0f;
                    touchPartial = 0.0f;
                    factoryReinforceSelected = null;
                    //check if the destination is a factory --> launch order
                    manageDestinationFactory();
                    break;

                case GestureType.Tap:

                    /*
                     * Implementare
                     */
                    break;
            }
        }
    }


  }
}

