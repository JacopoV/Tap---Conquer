using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using StateGameLayer;

namespace TapAndConquer3D
{
    public class TroopState : FactoryState
    {
        //getire collisione truppe-pianeta non target
        //getire collisione truppe-truppe alleate
        //getire collisione truppe-target
        //battglia

        StateGame stateGame;
        static private int troopRegenHitPoints = 10;
        static private int troopStatsFactor = 5;

        public TroopState(ContentManager Content)
            :base(Content)
        {
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
        }

        public List<StateGameLayer.Troop> getTroops()
        {
            return stateGame.troops;
        }

        public void addTroop(int owner, Vector3 position, int elements, StateGameLayer.Factory targetFactory, int maxHitPoints, int damage, int defense, int shields)
        {
            if (owner == 1 || owner == 2)
                stateGame.troops.Add(new StateGameLayer.Troop(owner, position, elements, targetFactory, maxHitPoints / troopStatsFactor, damage / troopStatsFactor, defense / troopStatsFactor, shields / troopStatsFactor));
        }

        public void setFellow()
        {
            //TODO
            //stateGame.troops[stateGame.troops.Count - 2].Fellow = stateGame.troops[stateGame.troops.Count - 1];
            //stateGame.troops[stateGame.troops.Count - 1].Fellow = stateGame.troops[stateGame.troops.Count - 2];
        }


        public bool checkCollision(StateGameLayer.Troop me, RenderingState renderingState, StateGameLayer.Troop enemy)
        {
            float distance = Vector3.Distance(me.CurrentPos, enemy.CurrentPos);
            return distance < 200;
        }

        private bool checkTroopsCollision(RenderingState renderingState, StateGameLayer.Troop toCheck) 
        {
            foreach (StateGameLayer.Troop i in stateGame.troops)
            {
                if(i.ObjectType != toCheck.ObjectType && checkCollision(toCheck, renderingState, i))
                {
                    toCheck.Fighting = true;
                    toCheck.StartTime = 0;
                    toCheck.EnemyEngaged = true;
                    toCheck.EnemyTroop = i;
                    ++toCheck.EnemyTroop.SmartEnemyCounter;
                }
            }
            return false;
        }

        public void manageArrival(StateGameLayer.Troop troop)
        {
            if (!troop.EnemyTroop.isDead())
            {
                int beforeBattleTroopHP = troop.HitPoints;
                int beforeBattleEnemyHP = troop.EnemyTroop.HitPoints;

                int attackTroop = Math.Max(troop.Damage - troop.EnemyTroop.Defense, 0);
                int shieldsEnemy = troop.EnemyTroop.Shields;
                troop.EnemyTroop.Shields = Math.Max(troop.EnemyTroop.Shields - attackTroop, 0);
                attackTroop = Math.Max(attackTroop - shieldsEnemy, 0);
                troop.EnemyTroop.HitPoints = troop.EnemyTroop.HitPoints - attackTroop;

                int attackEnemy = Math.Max(troop.EnemyTroop.Damage - troop.Defense, 0);
                int shieldsTroop = troop.Shields;
                troop.Shields = Math.Max(troop.Shields - attackEnemy, 0);
                attackEnemy = Math.Max(attackEnemy - shieldsTroop, 0);
                troop.HitPoints = troop.HitPoints - attackEnemy;

                troop.EnemyTroop.recalculateElements(beforeBattleEnemyHP);
                troop.recalculateElements(beforeBattleTroopHP);

                troop.EnemyTroop.StartTime = 0;
            }
            if (troop.EnemyTroop.isDead())
            {

                removeLaser(troop.EnemyTroop);

                troop.Fighting = false;
                troop.EnemyTroop = null;
                troop.EnemyEngaged = false;
            }
        }

        public override void regen()
        {
            base.regen();
            foreach (StateGameLayer.Troop i in stateGame.troops)
            {
                if (!i.Fighting)
                {
                    if (i.HitPoints < i.MaxHitPoints)
                    {
                        int futureHitPoints = i.HitPoints + troopRegenHitPoints;
                        i.HitPoints = Math.Min(futureHitPoints, i.MaxHitPoints);
                    }
                }
            }
        }

        //check if troop collision with another factory
        public float troopCollisionFactoryNoTarget(StateGameLayer.Troop me, RenderingState renderingState, List<StateGameLayer.Factory> factories, Vector3 posCurrent)
        {
            for (int i = 0; i < factories.Count; i++)
            {
                if (factories[i] != me.myTarget)
                {
                    BoundingSphere boundingSphere = renderingState.BoundingSpherePlanet;
                    renderingState.boundingSphereModelShipRed.Center = me.CurrentPos;

                    //calculate bounding sphere model planet
                    boundingSphere.Center = factories[i].CurrentPos;
                    boundingSphere.Radius *= factories[i].scaleFactor;

                    if (renderingState.boundingSphereModelShipRed.Intersects(boundingSphere))
                        if (Vector3.Distance(boundingSphere.Center, posCurrent) > boundingSphere.Radius + renderingState.boundingSphereModelShipRed.Radius + 1)
                            return boundingSphere.Radius;
                }
            }

            return 0;
        }


        public bool isArrivedAtTarget(StateGameLayer.Troop me, RenderingState renderingState)
        {
            //return (Vector3.Distance(CurrentPos, myTarget.CurrentPos) == 0);
            BoundingSphere boundingSphere = renderingState.BoundingSpherePlanet;
            //calculate bounding sphere model ship
            renderingState.boundingSphereModelShipRed.Center = me.CurrentPos;

            //calculate bounding sphere model planet
            boundingSphere.Center = me.myTarget.CurrentPos;

            //if collision
            boundingSphere.Radius *= me.myTarget.scaleFactor;
            return renderingState.boundingSphereModelShipRed.Intersects(boundingSphere);
        }

        public void update(float deltaT, RenderingState renderingState)
        {
            base.update(deltaT);

            for (int i = 0; i < stateGame.troops.Count; ++i)
            {
                if (!stateGame.troops[i].isDead())
                {
                    bool troopBattleFinished = false;
                    if (stateGame.troops[i].Fighting)
                    {
                        if (stateGame.troops[i].EnemyEngaged && stateGame.troops[i].EnemyTroop == null)
                        {
                            stateGame.troops[i].Fighting = false;
                            stateGame.troops[i].StartTime = 0;
                        }
                        else 
                        {
                            stateGame.troops[i].StartTime += deltaT;
                            if (stateGame.troops[i].StartTime >= stateGame.troops[i].TimeLimit)
                            {
                                troopBattleFinished = true;
                                stateGame.troops[i].StartTime = 0;
                            }
                        }                        
                    }
                    else if (stateGame.troops[i].SmartEnemyCounter <= 0)
                    {
                        if (stateGame.troops[i].Shields < stateGame.troops[i].MaxShields)
                        {
                            stateGame.troops[i].StartTime += deltaT;
                            if (stateGame.troops[i].StartTime >= stateGame.troops[i].RegenTime)
                            {
                                stateGame.troops[i].Shields = stateGame.troops[i].MaxShields;
                                stateGame.troops[i].StartTime = 0;
                            }
                        }

                        if (isArrivedAtTarget(stateGame.troops[i], renderingState))
                        {
                            bool finish = true;
                            //TODO
                            //if (stateGame.troops[i].Fellow != null)
                            //{
                            //    finish = false;
                            //    if (stateGame.troops[i].Target.ObjectType == stateGame.troops[i].ObjectType)// || troops[i].Target.ObjectType == 0)
                            //    {
                            //        finish = true;
                            //        stateGame.troops[i].Fellow.Fellow = null;
                            //    }
                            //    else
                            //    {
                            //        if (!stateGame.troops[i].FellowArrived)
                            //            stateGame.troops[i].Fellow.FellowArrived = true;
                            //        else
                            //        {
                            //            if (stateGame.troops[i].Elements > 0)
                            //            {
                            //                stateGame.troops[i].Fellow.FellowArrived = true;
                            //                stateGame.troops[i].Elements += stateGame.troops[i].Fellow.Elements;
                            //                stateGame.troops[i].Fellow.Elements = 0;
                            //            }
                            //            finish = true;
                            //        }
                            //    }
                            //}
                            if (finish)
                            {
                                if (stateGame.troops[i].Target.ObjectType != stateGame.troops[i].ObjectType)// && troops[i].Target.ObjectType != 0)
                                {
                                    stateGame.troops[i].Target.attack();
                                    stateGame.troops[i].Fighting = true;
                                    stateGame.troops[i].StartTime = 0;
                                }
                                else
                                    troopBattleFinished = true;
                            }
                        }
                        else
                        {

                            if (stateGame.troops[i].DimensionPlanetCollision == 0)
                            {
                                stateGame.troops[i].DimensionPlanetCollision = troopCollisionFactoryNoTarget(stateGame.troops[i], renderingState, stateGame.getFactories(), stateGame.troops[i].posOrigin);
                            }

                            if (stateGame.troops[i].DimensionPlanetCollision > 0 || stateGame.troops[i].CurrentPos.Y > 0)
                            {
                                if (stateGame.troops[i].CurrentPos.Y < stateGame.troops[i].DimensionPlanetCollision + renderingState.boundingSphereModelShipRed.Radius)
                                {
                                    stateGame.troops[i].CurrentPos = new Vector3(stateGame.troops[i].CurrentPos.X, stateGame.troops[i].CurrentPos.Y + stateGame.troops[i].Speed * deltaT, stateGame.troops[i].CurrentPos.Z);
                                    stateGame.troops[i].RunAbovePlanet = stateGame.troops[i].CurrentPos;
                                }
                                else if (Vector3.Distance(stateGame.troops[i].CurrentPos, stateGame.troops[i].RunAbovePlanet) < stateGame.troops[i].DimensionPlanetCollision * 2 + renderingState.boundingSphereModelShipRed.Radius * 2)
                                {
                                    stateGame.troops[i].CurrentPos = new Vector3(stateGame.troops[i].CurrentPos.X + stateGame.troops[i].Velocity.X * stateGame.troops[i].Speed * deltaT, stateGame.troops[i].CurrentPos.Y, stateGame.troops[i].CurrentPos.Z + stateGame.troops[i].Velocity.Z * stateGame.troops[i].Speed * deltaT);
                                }
                                else
                                {
                                    stateGame.troops[i].DimensionPlanetCollision = 0;
                                    stateGame.troops[i].CurrentPos = new Vector3(stateGame.troops[i].CurrentPos.X, stateGame.troops[i].CurrentPos.Y - stateGame.troops[i].Speed * deltaT, stateGame.troops[i].CurrentPos.Z);
                                    if (stateGame.troops[i].CurrentPos.Y <= 0)
                                    {
                                        stateGame.troops[i].RunAbovePlanet = Vector3.Zero;
                                        stateGame.troops[i].CurrentPos = new Vector3(stateGame.troops[i].CurrentPos.X, 0, stateGame.troops[i].CurrentPos.Z);
                                    }

                                }

                            }
                            else
                            {
                                stateGame.troops[i].Rotation = Math.Atan2(stateGame.troops[i].Target.CurrentPos.Z - stateGame.troops[i].CurrentPos.Z, stateGame.troops[i].Target.CurrentPos.X - stateGame.troops[i].CurrentPos.X);
                                stateGame.troops[i].Velocity = new Vector3((float)Math.Cos(stateGame.troops[i].Rotation), 0, (float)Math.Sin(stateGame.troops[i].Rotation));
                                stateGame.troops[i].CurrentPos = new Vector3(stateGame.troops[i].CurrentPos.X + stateGame.troops[i].Velocity.X * stateGame.troops[i].Speed * deltaT, stateGame.troops[i].CurrentPos.Y, stateGame.troops[i].CurrentPos.Z + stateGame.troops[i].Velocity.Z * stateGame.troops[i].Speed * deltaT);
                            }

                            stateGame.troops[i].Center = stateGame.troops[i].CurrentPos;
                            checkTroopsCollision(renderingState, stateGame.troops[i]);
                        }

                    }
                    bool deadExplosion = true;
                    if (troopBattleFinished)
                    {
                        if (stateGame.troops[i].EnemyTroop != null)
                        {

                            /*
                             * engage battle, laser starts
                             */

                            manageArrival(stateGame.troops[i]);

                            if (stateGame.troops[i].CurrentPos != null)
                            {
                                addLaser(stateGame.troops[i].CurrentPos, stateGame.troops[i]);
                            }
                            if (stateGame.troops[i].EnemyTroop != null)
                            {
                                addLaser(stateGame.troops[i].EnemyTroop.CurrentPos, stateGame.troops[i]);
                            }
                            

                            

                            
                            


                            //controlla che anke la truppa che stavo combattendo non sia morta nel frattempo (fai azioni come se fosse appena morta)
                        }
                        else 
                        {
                            StateGameLayer.Factory target = stateGame.troops[i].Target;
                            int nFactory = search(target);

                            

                            if (nFactory != -1) manageArrival(stateGame.troops[i], nFactory);

                            /*
                            * engage battle, laser starts
                            */
                            if (stateGame.troops[i].CurrentPos != null)
                            {
                                addLaser(stateGame.troops[i].CurrentPos, stateGame.troops[i]);
                            }

                            
                            

                            if (nFactory != -1)
                                deadExplosion = manageArrival(stateGame.troops[i], nFactory);

                        }                        
                    }
                    if (stateGame.troops[i].isDead())
                    {
                     /*
                     * add new explosion at troop posizion (VERIFY IF UNPROJECT IS NEEDED)
                     * 
                     */
                        //TODO
                        //if(stateGame.troops[i].Fellow != null)
                        //    stateGame.troops[i].Fellow.Fellow = null;
                        
                        removeLaser(stateGame.troops[i]);
                        if (stateGame.troops[i].EnemyTroop != null)
                        {
                            removeLaser(stateGame.troops[i].EnemyTroop);
                        }
                          
                       if (deadExplosion)
                            addExplosion(stateGame.troops[i].CurrentPos);                        


                        if (stateGame.troops[i].EnemyTroop != null)
                            --stateGame.troops[i].EnemyTroop.SmartEnemyCounter;

                        stateGame.troops.Remove(stateGame.troops[i]);
                        i--;
                    }
                }
                else
                {
                    /*
                     * add new explosion at troop posizion (VERIFY IF UNPROJECT IS NEEDED)
                     * 
                     */
                    //TODO
                    //if (stateGame.troops[i].Fellow != null)
                    //    stateGame.troops[i].Fellow.Fellow = null;
                    
                    removeLaser(stateGame.troops[i]);
                    if (stateGame.troops[i].EnemyTroop != null)
                    {
                        removeLaser(stateGame.troops[i].EnemyTroop);
                    }
                     

                    addExplosion(stateGame.troops[i].CurrentPos);

                    if (stateGame.troops[i].EnemyTroop != null)
                        --stateGame.troops[i].EnemyTroop.SmartEnemyCounter;

                    stateGame.troops.Remove(stateGame.troops[i]);
                    i--;
                }
            }
        }
    }
}
