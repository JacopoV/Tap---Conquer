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
    public class FactoryState : ExplosionState
    {
        public StateGame stateGame;

        public FactoryState(ContentManager Content)
            :base(Content)
        {
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
        }

        public int search(StateGameLayer.Factory searchTarget)
        {
            for (int i = 0; i < stateGame.factory.factories.Count; ++i)
            {
                if (stateGame.factory.factories[i].compareFactory(searchTarget))
                    return i;
            }
            return -1;
        }

        public bool manageArrival(StateGameLayer.Troop troop, int Target)
        {
            bool troopDie = true;
            if (troop.ObjectType != stateGame.factory.factories[Target].ObjectType)
            {
                int beforeBattleFactoryHP = stateGame.factory.factories[Target].HitPoints;
                int beforeBattleTroopHP = troop.HitPoints;

                int attackTroop = Math.Max(troop.Damage - stateGame.factory.factories[Target].Defense, 0);
                int shieldsFactory = stateGame.factory.factories[Target].Shields + (stateGame.factory.factories[Target].Shields / 20) * stateGame.factory.factories[Target].Elements;
                stateGame.factory.factories[Target].Shields = Math.Max(stateGame.factory.factories[Target].Shields - attackTroop, 0);
                attackTroop = Math.Max(attackTroop - shieldsFactory, 0);
                stateGame.factory.factories[Target].HitPoints = stateGame.factory.factories[Target].HitPoints - attackTroop;

                int attackFactory = Math.Max(stateGame.factory.factories[Target].Damage + (stateGame.factory.factories[Target].Damage / 20) * stateGame.factory.factories[Target].Elements - troop.Defense, 0);
                int shieldsTroop = troop.Shields;
                troop.Shields = Math.Max(troop.Shields - attackFactory, 0);
                attackFactory = Math.Max(attackFactory - shieldsTroop, 0);
                troop.HitPoints = troop.HitPoints - attackFactory;

                stateGame.factory.factories[Target].recalculateElements(beforeBattleFactoryHP);
                troop.recalculateElements(beforeBattleTroopHP);

                if (stateGame.factory.factories[Target].isDead() && !troop.isDead())
                {
                    stateGame.factory.factories[Target].Elements = troop.Elements;
                    stateGame.factory.factories[Target].Elements = Math.Min(stateGame.factory.factories[Target].Elements, stateGame.factory.factories[Target].MaxElements);
                    stateGame.factory.factories[Target].ObjectType = troop.ObjectType;
                    stateGame.factory.factories[Target].HitPoints = stateGame.factory.factories[Target].MaxHitPoints;
                    stateGame.factory.factories[Target].Shields = stateGame.factory.factories[Target].MaxShields;
                    stateGame.factory.factories[Target].Selected = false;
                    troop.HitPoints = 0;
                    troopDie = false;
                    stateGame.factory.change = true;
                }
                else if (stateGame.factory.factories[Target].isDead() && troop.isDead())
                {
                    stateGame.factory.factories[Target].HitPoints = stateGame.factory.factories[Target].MaxHitPoints / 10;
                    troopDie = true;
                }
            }
            else
            {
                stateGame.factory.factories[Target].Elements += troop.Elements;
                stateGame.factory.factories[Target].Elements = Math.Min(stateGame.factory.factories[Target].Elements, stateGame.factory.factories[Target].MaxElements);
                troop.HitPoints = 0;
                troopDie = false;
            }
            if (troop.isDead())
            {
                stateGame.factory.factories[Target].UnderAttack = false;
                if(troopDie)
                    return true;
            }
            return false;
        }

        virtual public void regen()
        {
            foreach (StateGameLayer.Factory i in stateGame.factory.factories)
            {
                if (!i.UnderAttack)
                {
                    if (i.HitPoints < i.MaxHitPoints)
                    {
                        int futureHitPoints = i.HitPoints + stateGame.factory.factoryRegenHitPoints;
                        i.HitPoints = Math.Min(futureHitPoints, i.MaxHitPoints);
                    }
                }
            }
        }

        public new void update(float time)
        {
            base.update(time);

            foreach (StateGameLayer.Factory i in stateGame.factory.factories)
            {
                if (!i.UnderAttack)
                    i.StartTime += time;
                if (i.StartTime >= i.TimeLimit)
                {
                    i.StartTime = 0;
                    if (i.ObjectType != 0 && i.Elements < i.MaxElements)
                    {
                        i.Elements++;
                    }
                    if (i.Shields < i.MaxShields)
                    {
                        int regenFactor = (int)(i.RegenTime/i.TimeLimit);
                        if(regenFactor > 1)
                            i.Shields = Math.Min(i.MaxShields, i.Shields + i.MaxShields / (int)(i.RegenTime / i.TimeLimit));                            
                        else
                            i.Shields = i.MaxShields;
                    }
                }
            }
        }
    }
}
