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

namespace StateGameLayer
{
    //BonusPowerState state
    public class BonusPowerGame
    {
         public List<float> powers = new List<float>();
         public List<float> powersTime = new List<float>();
         public List<float> powersMaxTime = new List<float>();
        public int numberOfPowers = 7;
        public int seconds = 20;
        public int areasNumber = 4;

        public BonusPowerGame() { }
        public BonusPowerGame(bool newGame)
        {
            for (int i = 0; i < numberOfPowers; ++i)
            {
                powers.Add(-1);
                powersTime.Add(-1);
            }
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
            powersMaxTime.Add(5);
        }
    }

    // FactoryState state
    public class FactoryGame
    {
        public List<Factory> factories  = new List<Factory>();
        public int factoryMinNum = 8;
        public int factoryMaxNum = 12;
        public int xCoord = 5000;
        public int yCoord = 3500;
        public int capitalFactoryCap = 20;
        public int neutralFactoryMinCap = 2;
        public int neutralFactoryMaxCap = 15;

        public int neutralFactoryMinHitPoints = 2000;
        public int neutralFactoryMaxHitPoints = 5000;
        public int neutralFactoryMinDamage = 1000;
        public int neutralFactoryMaxDamage = 1500;
        public int neutralFactoryMinDefense = 500;
        public int neutralFactoryMaxDefense = 900;
        public int neutralFactoryMinShields = 1000;
        public int neutralFactoryMaxShields = 2000;

        public int factoryRegenHitPoints = 200;

        public bool change = false;

        public FactoryGame()
        { 
        
        }

        public FactoryGame(bool generate)
        {
            GenerateFactories();
        }

        public void GenerateFactories()
        {
            Random generation = new System.Random();
           
            List<Vector3> coordFactory = new List<Vector3>();

            //level
            for (int i = -yCoord; i < yCoord; i += 2000)
            {
                for (int j = -xCoord; j < xCoord; j += 3000)
                {
                    coordFactory.Add(new Vector3(j, 0, i));
                }
            }

            int factoryNum = generation.Next(factoryMinNum, factoryMaxNum);

            for (int i = 0; i < factoryNum; i++)
            {
                int hitPointsFactory = generation.Next(neutralFactoryMinHitPoints, neutralFactoryMaxHitPoints);
                int damageFactory = generation.Next(neutralFactoryMinDamage, neutralFactoryMaxDamage);
                int defenseFactory = generation.Next(neutralFactoryMinDefense, neutralFactoryMaxDefense);
                int shieldsFactory = generation.Next(neutralFactoryMinShields, neutralFactoryMaxShields);
                int numberCoord;
                if(i < 8)
                    numberCoord = generation.Next(14 - i*2, 16 - i*2);
                else
                    numberCoord = generation.Next(0, coordFactory.Count);

                int textureType = generation.Next(0, 7);
                int areaFactory = ((((int)coordFactory[numberCoord].X + xCoord) / xCoord) + 1) + 2 * ((((int)coordFactory[numberCoord].Z + yCoord) / yCoord));
                int scaleFactor = 4;

                if (damageFactory > 1150 && damageFactory <= 1350)
                    scaleFactor = 5;
                else if (damageFactory > 1350 && damageFactory <= 1500)
                    scaleFactor = 7;

                if (i == 0)
                {
                    factories.Add(new Factory(1, coordFactory[numberCoord], capitalFactoryCap, areaFactory, hitPointsFactory, damageFactory, defenseFactory, shieldsFactory, areaFactory, "planet", scaleFactor));
                }
                else if (i == factoryNum-1)
                {
                    factories.Add(new Factory(2, coordFactory[numberCoord], capitalFactoryCap, areaFactory, hitPointsFactory, damageFactory, defenseFactory, shieldsFactory, areaFactory, "planet", scaleFactor));
                }
                else
                {
                    int capFactory = generation.Next(neutralFactoryMinCap, neutralFactoryMaxCap);
                    factories.Add(new Factory(0, coordFactory[numberCoord], capFactory, areaFactory, hitPointsFactory, damageFactory, defenseFactory, shieldsFactory, areaFactory, "planet", scaleFactor));
                }
                coordFactory.RemoveAt(numberCoord);
            }
        }
    
    }

    // IAState state
    public class AI
    { 
     public List<Factory> myFactories  = new List<Factory>();
     public List<Factory> neutralFactories = new List<Factory>();
     public List<Factory> enemyFactories = new List<Factory>();

     public List<Factory> mineSelected = new List<Factory>();
     public List<Factory> neutralSelected = new List<Factory>();
     public List<Factory> enemySelected = new List<Factory>();

     public float timer = 0;

     public AI()
        {
        }
    }

    //GameState  state
    public class State
    {

        public List<Order> player1_orders = new List<Order>();
        public List<Order> player2_orders = new List<Order>();

        public bool touchBegin = false;

        public Factory factoryReinforceSelected = null;

        public float lastTurn = 0;

        public float? previousDistance;
        public List<Vector2> dragging = new List<Vector2>();

        public double totalGameTime;
        public int score;
        public bool enabled = true;


        public State() {}
        public State(bool newGame)
        {
            totalGameTime = 0.0;
            score = 0;
        }

        //ADD NEW ORDER
        public bool addOrder(List<StateGameLayer.Factory> newOrder, int player)
        {
            if (enabled)
            {
                if (player == 1 && player1_orders.Count < 3)
                {
                    player1_orders.Add(new StateGameLayer.Order(newOrder));
                    if (player1_orders.Count < 3)
                        return true;
                }
                else if (player == 2 && player2_orders.Count < 3)
                {
                    player2_orders.Add(new StateGameLayer.Order(newOrder));
                    if (player2_orders.Count < 3)
                        return true;
                }
            }
            return false;
        }

    }

    //This class is the global state of the game
    public class StateGame
    {
        public State state;
        public List<Troop> troops = new List<Troop>();
        public AI ia = new AI();
        public FactoryGame factory;
        public List<Laser> lasers = new List<Laser>();
        public List<Explosion> explosions = new List<Explosion>();
        public BonusPowerGame bonus;

        public StateGame()
        {
            factory = new FactoryGame();
            bonus = new BonusPowerGame();
            state = new State();
        }

        public StateGame(bool newStart)
        {
            factory = new FactoryGame(true);
            bonus = new BonusPowerGame(true);
            state = new State(true);
        }

        public List<Factory> getFactories()
        {
            return factory.factories;
        }
    }
}
