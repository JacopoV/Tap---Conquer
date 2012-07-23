using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using StateGameLayer;
using Microsoft.Xna.Framework.Content;

namespace TapAndConquer3D
{
    public class IAState: TroopState
    {
        //GameState m;

        const int TURN_SECONDS = 3;
        public StateGame stateGame;


        public IAState(ContentManager Content)
            :base(Content)
        {
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
        }

        public void updateIAInfo(float deltaT)
        {
            stateGame.ia.myFactories.Clear();
            stateGame.ia.enemyFactories.Clear();

            /*
             * Factory types:
             * 
             * 0 = Neutral
             * 1 = Human Player
             * 2 = AI
             */
            foreach (StateGameLayer.Factory i in stateGame.getFactories())
            {
                if (i.ObjectType == 1)
                    stateGame.ia.enemyFactories.Add(i);

                if (i.ObjectType == 2)
                    stateGame.ia.myFactories.Add(i);
            }

            /*
             * Every X seconds update info about the state of factories
             */
            stateGame.ia.timer += deltaT;
            if (stateGame.ia.timer > 8)
            {
                stateGame.ia.mineSelected.Clear();
                stateGame.ia.mineSelected = stateGame.ia.myFactories;

                stateGame.ia.enemySelected.Clear();

                stateGame.ia.timer = 0;
            }
        }

        //public void setIAGameState(GameState gs)
        //{
        //    m = gs;
        //}

        public new void update(float deltaT, RenderingState renderingState)
        {
            updateIAInfo(deltaT);

            /*
             * If AI has at least one factory, AI can choose what to do
             */
            if (stateGame.ia.myFactories.Count > 0)   
            {
                /*
                 * Beginning strategy: conquer neutral factories
                 */
                bool noMoreActions = false;
                while (stateGame.ia.neutralFactories.Count != 0 && !noMoreActions)
                {
                    /*If beginning strategy succeded useBeginningStrategy returns true. 
                     * If true use beginningStrategy until there are orders
                     * If false AI turn is finished and returns to manager
                     */
                    if (useBeginningStrategy(deltaT) == false)
                        noMoreActions = true;

                }

                /*
                 * Standard strategy: conquer weakest enemy factories from nearest AI factories
                 */

                if (useStandardStrategy() == false)
                    noMoreActions = true;

            }

            base.update(deltaT, renderingState);
        }

        private bool useBeginningStrategy(float deltaT)
        {
            /*
             * Choose from AI factories those with sufficient troop number (>1)
             */
            List<StateGameLayer.Factory> origin = new List<StateGameLayer.Factory>();
            foreach (StateGameLayer.Factory i in stateGame.ia.myFactories)
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
             * Make sure neutralFactories only contains neutral factories 
             * (maybe the enemy conquered one?)
             */
            List<StateGameLayer.Factory> target = stateGame.ia.neutralFactories;

            for (int i = 0; i < stateGame.ia.neutralFactories.Count; i++)
            {
                if (stateGame.ia.neutralFactories[i].ObjectType != 0)
                    target.Remove(stateGame.ia.neutralFactories[i]);
            }

            stateGame.ia.neutralFactories = target;

            if (target.Count == 0)
                return false;

            /* Find nearest couple between my factories (from wich AI has to originate the attack)
             * and neutral targetable factories
             */
            List<StateGameLayer.Factory> result = findNearestFactoryCouple(origin, target);

            if (result.Count < 2)
            {
                /*error!*/
                return false;
            }
            /*
            * Remember to AI that the neutral factory "result[1]" has already been targeted.
            */
            stateGame.ia.neutralFactories.Remove(result[1]);

            /*
             * Tell the manager to add this new order.
             * If after this order there are no more spaces to add orders, return false.
             * If AI can make other actions, return true.
             */
            return stateGame.state.addOrder(result, 2);
        }

        private bool useStandardStrategy()
        {

            /*
             * Remove from targetable enemy factories those which AI has already attacked
             */
            List<StateGameLayer.Factory> targetable = new List<StateGameLayer.Factory>();

            foreach (StateGameLayer.Factory i in stateGame.ia.enemyFactories)
            {
                if (!stateGame.ia.enemySelected.Contains(i))
                    targetable.Add(i);
            }

            /*
             * Find the weakest enemy factory.
             */
            StateGameLayer.Factory weakest = null;
            float min = 10000;
            foreach (StateGameLayer.Factory i in targetable)
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
                StateGameLayer.Factory nearest = findNearestFactory(weakest, stateGame.ia.myFactories);

                /*
                 * Check if that factory can attack the weakest with a "single attack"
                 */
                if (nearest.Elements > weakest.Elements * 1.2f)
                {
                    List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                    order.Add(nearest);
                    order.Add(weakest);

                    stateGame.state.addOrder(order, 2);

                    /*Remember the factory AI has just attacked*/
                    stateGame.ia.enemySelected.Add(weakest);
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
                    if (stateGame.ia.myFactories.Count > 1)
                    {
                        /* 
                         * Search for the second nearest to the weakest
                         */
                        stateGame.ia.myFactories.Remove(nearest);

                        StateGameLayer.Factory helping = null;
                        bool found = false;
                        for (int i = 0; i < stateGame.ia.myFactories.Count && !found; i++)
                        {
                            helping = findNearestFactory(weakest, stateGame.ia.myFactories);
                            if (helping.Elements + nearest.Elements > weakest.Elements)
                            {
                                found = true;
                            }
                            else
                            {
                                if (stateGame.ia.myFactories.Count > 1)
                                    stateGame.ia.myFactories.Remove(helping);
                                else
                                    found = true;
                            }
                        }

                        if (found)
                        {
                            /*Dispatch the double attack */
                            List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                            order.Add(nearest);
                            order.Add(helping);
                            order.Add(weakest);

                            return stateGame.state.addOrder(order, 2);

                        }


                    }

                }
            }

            /*If here the enemy is a looser*/
            return false;

        }

        private List<StateGameLayer.Factory> findNearestFactoryCouple(List<StateGameLayer.Factory> origin, List<StateGameLayer.Factory> target)
        {
            /*
             * From two List<Factory>, search for the nearest couple of factories
             * 
             */
            List<StateGameLayer.Factory> result = new List<StateGameLayer.Factory>();

            float min = 10000;
            foreach (StateGameLayer.Factory i in origin)
            {
                foreach (StateGameLayer.Factory j in target)
                {
                    if (Vector3.Distance(i.Center, j.Center) < min)
                    {
                        result.Clear();

                        min = Vector3.Distance(i.Center, j.Center);
                        result.Add(i);
                        result.Add(j);

                    }
                }
            }

            return result;
        }

        private StateGameLayer.Factory findNearestFactory(StateGameLayer.Factory start, List<StateGameLayer.Factory> searchList)
        {
            /*
             * Search in "searchList" for the nearest factory to "start". 
             */
            float min = 100000;
            StateGameLayer.Factory nearestFactory = null;

            foreach (StateGameLayer.Factory i in searchList)
            {
                if (Vector3.Distance(start.Center, i.Center) < min)
                {
                    min = Vector3.Distance(start.Center, i.Center);
                    nearestFactory = i;
                }
            }

            return nearestFactory;
        }

        private StateGameLayer.Factory findFurthestFactory(StateGameLayer.Factory start, List<StateGameLayer.Factory> searchList)
        {
            float max = 0;
            StateGameLayer.Factory furthestFactory = start;

            foreach (StateGameLayer.Factory i in searchList)
            {
                if (Vector3.Distance(start.Center, i.Center) > max)
                {
                    max = Vector3.Distance(start.Center, i.Center);
                    furthestFactory = i;
                }
            }

            return furthestFactory;
        }

        private int factoriesNumber(int type)
        {
            int num = 0;
            foreach (StateGameLayer.Factory i in stateGame.getFactories())
            {
                if (i.ObjectType == type)
                    num++;
            }

            return num;
        }


        public void loadNeutralFactories()
        {
            foreach (StateGameLayer.Factory i in stateGame.getFactories())
            {
                if (i.ObjectType == 0)
                    stateGame.ia.neutralFactories.Add(i);
            }
        }

    }
}

