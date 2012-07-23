using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TapAndConquer.GameObjects;

namespace TapAndConquer
{
    public class IA
    {
        GameState m;
        private List<Factory> myFactories;
        private List<Factory> neutralFactories;
        private List<Factory> enemyFactories;

        private List<Factory> mineSelected;
        private List<Factory> neutralSelected;
        private List<Factory> enemySelected;

        const int TURN_SECONDS = 5;
        private float timer = 0;

        public IA(GameState manager)
        {
            m = manager;
            myFactories = new List<Factory>();
            neutralFactories = new List<Factory>();
            enemyFactories = new List<Factory>();

            neutralSelected = new List<Factory>();
            mineSelected = new List<Factory>();
            enemySelected = new List<Factory>();
        }

        public void updateInfo(float deltaT)
        {
            myFactories.Clear();
            enemyFactories.Clear();

            /*
             * Factory types:
             * 
             * 0 = Neutral
             * 1 = Human Player
             * 2 = AI
             */
            foreach (Factory i in m.getFactories())
            {
                if (i.ObjectType == 1)
                    enemyFactories.Add(i);

                if (i.ObjectType == 2)
                    myFactories.Add(i);
            }

            /*
             * Every X seconds update info about the state of factories
             */
            timer += deltaT;
            if (timer > 8)
            {
                mineSelected.Clear();
                mineSelected = myFactories;

                enemySelected.Clear();

                timer = 0;
            }
        }

        public void useIA(float deltaT)
        {
            updateInfo(deltaT);

            /*
             * If AI has at least one factory, AI can choose what to do
             */
            if (myFactories.Count > 0)
            {
                /*
                 * Beginning strategy: conquer neutral factories
                 */
                while (neutralFactories.Count / 2 != 0)
                {
                    /*If beginning strategy succeded useBeginningStrategy returns true. 
                     * If true use beginningStrategy until there are orders
                     * If false AI turn is finished and returns to manager
                     */
                    if (useBeginningStrategy(deltaT) == false)
                        return;

                }

                /*
                 * Standard strategy: conquer weakest enemy factories from nearest AI factories
                 */

                if (useStandardStrategy() == false)
                    return;

            }
        }

        private bool useBeginningStrategy(float deltaT)
        {
            /*
             * Choose from AI factories those with sufficient troop number (>1)
             */
            List<Factory> origin = new List<Factory>();
            foreach (Factory i in myFactories)
            {
                if (i.Elements > 1)
                {
                    origin.Add(i);
                }
            }

            /* Check if there is at least one factory from which AI can attack*/
            if (origin.Count == 0)
                return false;

            /*
             * Find the neutral factories AI can attack. 
             * Make sure neutralFactories only contains neutral factories (maybe the enemy conquered one?)
             */
            List<Factory> target = neutralFactories;

            for (int i = 0; i < neutralFactories.Count; i++ )
            {
                if (neutralFactories[i].ObjectType != 0)
                    target.Remove(neutralFactories[i]);
            }

            neutralFactories = target;

            if (target.Count == 0)
                return false;

            /* Find nearest couple between my factories (from wich AI has to originate the attack)
             * and neutral targetable factories
             */
            List<Factory> result = findNearestFactoryCouple(origin, target);

            /*
            * Remember to AI that the neutral factory "result[1]" has already been targeted.
            */
            neutralFactories.Remove(result[1]);

            /*
             * Tell the manager to add this new order.
             * If after this order there are no more spaces to add orders, return false.
             * If AI can make other actions, return true.
             */
            return m.addOrder(result, 2);
        }

        private bool useStandardStrategy()
        {

            /*
             * Remove from targetable enemy factories those which AI has already attacked
             */
            List<Factory> targetable = new List<Factory>();

            foreach (Factory i in enemyFactories)
            {
                if (!enemySelected.Contains(i))
                    targetable.Add(i);
            }

            /*
             * Find the weakest enemy factory.
             */
            Factory weakest = null;
            float min = 10000;
            foreach (Factory i in targetable)
            {
                if (i.Elements < min)
                {
                    weakest = i;
                    min = weakest.Elements;
                }
            }

            /*
             * If there is a weakest factory...
             * 
             */
            if (weakest != null)
            {
                /*
                 * ...search into AI factories for the nearest to the weakest.
                 */
                Factory nearest = findNearestFactory(weakest, myFactories);

                /*
                 * Check if that factory can attack the weakest with a "single attack"
                 */
                if (nearest.Elements > weakest.Elements * 1.2f)
                {
                    List<Factory> order = new List<Factory>();
                    order.Add(nearest);
                    order.Add(weakest);

                    m.addOrder(order, 2);

                    /*Remember the factory AI has just attacked*/
                    enemySelected.Add(weakest);
                }
                else
                {
                    /*
                     * ...if "single attack" did not succeded...
                     * */
                    
                    /*
                     * Check if AI has at least two factory 
                     * in order to dispatch a "double attack"
                     */
                    if (myFactories.Count > 1)
                    {
                        /* 
                         * Search for the second nearest to the weakest
                         */
                        myFactories.Remove(nearest);

                        Factory helping = null;
                        bool found = false;
                        for (int i = 0; i < myFactories.Count && !found; i++)
                        {
                            helping = findNearestFactory(weakest, myFactories);
                            if (helping.Elements + nearest.Elements > weakest.Elements)
                            {
                                found = true;
                            }
                            else
                            {
                                if (myFactories.Count > 1)
                                    myFactories.Remove(helping);
                                else
                                    found = true;
                            }
                        }

                        if (found)
                        {
                            /*Dispatch the double attack */
                            List<Factory> order = new List<Factory>();
                            order.Add(nearest);
                            order.Add(helping);
                            order.Add(weakest);

                            return m.addOrder(order, 2);

                        }


                    }

                }
            }

            /*If here the enemy is a looser*/
            return false;

        }

        private List<Factory> findNearestFactoryCouple(List<Factory> origin, List<Factory> target)
        {
            /*
             * From two List<Factory>, search for the nearest couple of factories
             * 
             */
            List<Factory> result = new List<Factory>();

            float min = 10000;
            foreach (Factory i in origin)
            {
                foreach (Factory j in target)
                {
                    if (Vector2.Distance(i.Center, j.Center) < min)
                    {
                        result.Clear();

                        min = Vector2.Distance(i.Center, j.Center);
                        result.Add(i);
                        result.Add(j);

                    }
                }
            }

            return result;
        }

        private Factory findNearestFactory(Factory start, List<Factory> searchList)
        {
            /*
             * Search in "searchList" for the nearest factory to "start". 
             */
            float min = 100000;
            Factory nearestFactory = null;

            foreach (Factory i in searchList)
            {
                if (Vector2.Distance(start.Center, i.Center) < min)
                {
                    min = Vector2.Distance(start.Center, i.Center);
                    nearestFactory = i;
                }
            }

            return nearestFactory;
        }

        private Factory findFurthestFactory(Factory start, List<Factory> searchList)
        {
            float max = 0;
            Factory furthestFactory = start;

            foreach (Factory i in searchList)
            {
                if (Vector2.Distance(start.Center, i.Center) > max)
                {
                    max = Vector2.Distance(start.Center, i.Center);
                    furthestFactory = i;
                }
            }

            return furthestFactory;
        }

        private int factoriesNumber(int type)
        {
            int num = 0;
            foreach (Factory i in m.getFactories())
            {
                if (i.ObjectType == type)
                    num++;
            }

            return num;
        }


        public void loadNeutralFactories()
        {
            foreach (Factory i in m.getFactories())
            {
                if (i.ObjectType == 0)
                    neutralFactories.Add(i);
            }
        }

    }
}