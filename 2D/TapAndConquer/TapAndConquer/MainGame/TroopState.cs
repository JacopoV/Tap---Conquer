using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TapAndConquer.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TapAndConquer
{
    public class TroopState : FactoryState
    {

        protected List<Troop> troops = new List<Troop>();

        public TroopState()
        {
        }

        public List<Troop> getTroops()
        {
            return troops;
        }

        public void addTroop(int owner, Vector2 position, int elements, Factory targetFactory)
        {
            if(owner == 1)
                troops.Add(new Troop(owner, position, elements, targetFactory));
            if (owner == 2)
                troops.Add(new Troop(owner, position, elements, targetFactory));
        }

        public void setFellow()
        {
            troops[troops.Count - 2].Fellow = troops[troops.Count - 1];
            troops[troops.Count - 1].Fellow = troops[troops.Count - 2];
        }

        public void update(float deltaT)
        {

            base.update(deltaT);

            for (int i = 0; i < troops.Count; ++i)
            {
                bool troopBattleFinished = false;
                if (troops[i].Fighting)
                {
                    troops[i].StartTime += deltaT;
                    if (troops[i].StartTime >= troops[i].TimeLimit)
                        troopBattleFinished = true;
                }
                else
                {
                    if (troops[i].isArrivedAtTarget())
                    {
                        bool finish = true;
                        if (troops[i].Fellow != null)
                        {
                            finish = false;
                            if (troops[i].Target.ObjectType == troops[i].ObjectType || troops[i].Target.ObjectType == 0)
                            {
                                finish = true;
                                troops[i].Fellow.Fellow = null;
                            }
                            else
                            {
                                if (!troops[i].FellowArrived)
                                    troops[i].Fellow.FellowArrived = true;
                                else
                                {
                                    if (troops[i].Elements > 0)
                                    {
                                        troops[i].Fellow.FellowArrived = true;
                                        troops[i].Elements += troops[i].Fellow.Elements;
                                        troops[i].Fellow.Elements = 0;
                                    }
                                    finish = true;
                                }
                            }
                        }
                        if (finish)
                        {
                            if (troops[i].Target.ObjectType != troops[i].ObjectType && troops[i].Target.ObjectType != 0)
                            {
                                troops[i].Target.attack();
                                troops[i].Fighting = true;  
                            }
                            else
                                troopBattleFinished = true;
                        }
                    }
                    else
                    {
                        troops[i].CurrentPos = new Vector2(troops[i].CurrentPos.X + troops[i].Velocity.X * troops[i].Speed * deltaT, troops[i].CurrentPos.Y + troops[i].Velocity.Y * troops[i].Speed * deltaT);
                        troops[i].Center = new Vector2(troops[i].CurrentPos.X + troops[i].Width * 0.5f, troops[i].CurrentPos.Y + troops[i].Height * 0.5f);
                    }

                    //return false;
                }



                if (troopBattleFinished)
                {
                    Factory target = troops[i].Target;
                    int nFactory = search(target);
                    if (nFactory != -1) manageArrival(troops[i], nFactory);
                    troops.Remove(troops[i]);
                    i--;
                }
            }

        }
    }

}
