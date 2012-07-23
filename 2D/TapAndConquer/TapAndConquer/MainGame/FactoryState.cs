using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TapAndConquer.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace TapAndConquer
{
    public class FactoryState
    {

        protected List<Factory> factories;
        private static int factoryMinNum = 8;
        private static int factoryMaxNum = 15;
        private static int capitalFactoryCap = 20;
        private static int neutralFactoryMinCap = 2;
        private static int neutralFactoryMaxCap = 15;

        public FactoryState()
        {   
            factories = new List<Factory>();

            Random generation = new System.Random();

            List<Vector2> coordFactory = new List<Vector2>();

            int numFactory = generation.Next(factoryMinNum, factoryMaxNum);

            int widthFactory = GraphicsDeviceManager.DefaultBackBufferWidth / 7;
            int heightFactory = GraphicsDeviceManager.DefaultBackBufferHeight / 3;

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
                    factories.Add(new Factory(1, coordFactory[posFactory], capitalFactoryCap));
                }
                else if (i == numFactory - 1)
                {
                    factories.Add(new Factory(2, coordFactory[posFactory], capitalFactoryCap));
                }
                else
                {
                    factories.Add(new Factory(0, coordFactory[posFactory], generation.Next(neutralFactoryMinCap, neutralFactoryMaxCap)));
                }

                coordFactory.Remove(coordFactory[posFactory]);
            }
        }

        public List<Factory> getFactories()
        {
            return factories;
        }

        public int search(Factory searchTarget)
        {
            for (int i = 0; i < factories.Count; ++i)
            {
                if (factories[i].compareFactory(searchTarget))
                    return i;
            }
            return -1;
        }

        public void manageArrival(Troop troop, int Target)
        {
            factories[Target].UnderAttack = false;
            if (troop.ObjectType != factories[Target].ObjectType)
            {
                if (troop.Elements > factories[Target].Elements)
                {
                    factories[Target].Elements = troop.Elements - factories[Target].Elements;
                    factories[Target].Elements = Math.Min(factories[Target].Elements, factories[Target].MaxElements);
                    factories[Target].ObjectType = troop.ObjectType;
                }
                else
                {
                    factories[Target].Elements -= troop.Elements;
                }
            }
            else
            {
                factories[Target].Elements += troop.Elements;
                factories[Target].Elements = Math.Min(factories[Target].Elements, factories[Target].MaxElements);
            }
        }

        public void update(float time)
        {

            foreach (Factory i in factories)
            {
                if (!i.UnderAttack)
                    i.StartTime += time;
                if (i.StartTime >= i.TimeLimit)
                {
                    if (i.ObjectType != 0 && i.Elements < i.MaxElements)
                    {
                        i.Elements++;
                        i.StartTime = 0;
                    }
                }
            }
        }
    }
}
