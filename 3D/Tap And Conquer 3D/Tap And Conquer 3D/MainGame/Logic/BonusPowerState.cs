using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StateGameLayer;
using Microsoft.Xna.Framework.Content;

namespace TapAndConquer3D
{
    public class BonusPowerState : IAState
    {
        public StateGame stateGame;

        public BonusPowerState(ContentManager Content)
            :base(Content)
        {
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
        }

        public List<float> getPowers()
        {
            return stateGame.bonus.powers;
        }

        public void activatePower(int number)
        {
            stateGame.bonus.powers[number] = 0;
            stateGame.bonus.powersTime[number] = 0;

            switch (number)
            {
                case 0:
                    foreach (StateGameLayer.Troop i in stateGame.troops)
                    {
                        if (i.ObjectType == 1)
                        {
                            i.Speed = i.Speed * 2.5f;
                        }
                    }
                    foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                    {
                        if (i.ObjectType == 1)
                        {
                            i.TimeLimit = i.TimeLimit / 2;
                        }
                    }
                    break;
                case 1:
                    foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                    {
                        if (i.ObjectType == 2)
                        {
                            i.TimeLimit = i.TimeLimit * 10000;
                        }
                    }
                    break;
                case 2:
                    foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                    {
                        if (i.ObjectType == 2)
                        {
                            i.Elements = i.Elements / 2;
                        }
                    }
                    break;
                case 3:
                    foreach (StateGameLayer.Troop i in stateGame.troops)
                    {
                        if (i.ObjectType == 2)
                        {
                            i.HitPoints = 0;
                            i.MaxHitPoints = 0;
                        }
                    }
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    break;
            }
        }

        public void deactivatePower(int number)
        {
            stateGame.bonus.powersTime[number] = -1;

            switch (number)
            {
                case 0:
                    foreach (StateGameLayer.Troop i in stateGame.troops)
                    {
                        if (i.ObjectType == 1)
                        {
                            i.Speed = i.Speed / 2.5f;
                        }
                    }
                    foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                    {
                        if (i.ObjectType == 1)
                        {
                            i.TimeLimit = i.TimeLimit * 2;
                        }
                    }
                    break;
                case 1:
                    foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                    {
                        if (i.ObjectType == 2)
                        {
                            i.TimeLimit = i.TimeLimit / 10000;
                        }
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    break;
            }
        }

        public new void update(float deltaT, RenderingState renderingState)
        {
            base.update(deltaT, renderingState);
            if (stateGame.factory.change)
            {
                stateGame.factory.change = false;
                List<bool> areas = new List<bool>();
                for (int i = 0; i < stateGame.bonus.numberOfPowers; ++i)
                {
                    if (i < stateGame.bonus.areasNumber) areas.Add(true);
                    else areas.Add(false);
                }

                foreach (StateGameLayer.Factory i in stateGame.factory.factories)
                {
                    if (i.ObjectType != 1)
                    {
                        areas[i.Area - 1] = false;
                    }
                }
                int areasCount = areas.Count(a => a == true);
                areas[4] = (areasCount >= 1);
                areas[5] = (areasCount >= 2);
                areas[6] = (areasCount >= 3);

                for (int i = 0; i < stateGame.bonus.numberOfPowers; ++i)
                {
                    if (areas[i] && stateGame.bonus.powers[i] == -1)
                    {
                        stateGame.bonus.powers[i] = 0;
                    }
                    else if (!areas[i])
                    {
                        stateGame.bonus.powers[i] = -1;
                    }
                }
            }

            List<int> timer = new List<int>();

            for (int i = 0; i < stateGame.bonus.numberOfPowers; ++i)
            {
                if (stateGame.bonus.powers[i] != -1)
                {
                    stateGame.bonus.powers[i] = Math.Min(stateGame.bonus.powers[i] + (deltaT / stateGame.bonus.seconds), 1);
                    timer.Add((int)(stateGame.bonus.powers[i] * 10));
                }
                else
                {
                    timer.Add(0);
                }
                if (stateGame.bonus.powersTime[i] != -1)
                {
                    stateGame.bonus.powersTime[i] = Math.Min(stateGame.bonus.powersTime[i] + deltaT, stateGame.bonus.powersMaxTime[i]);
                    if (stateGame.bonus.powersTime[i] == stateGame.bonus.powersMaxTime[i])
                    {
                        deactivatePower(i);
                    }
                }
            }
            renderingState.speedTimer = timer[0];
            renderingState.freezeTimer = timer[1];
            renderingState.halfFactoryTimer = timer[2];
            renderingState.armageddonTimer = timer[3];
            if (stateGame.bonus.powers[0] == 1) renderingState.isSpeedBonusActive = true;
            if (stateGame.bonus.powers[1] == 1) renderingState.isFreezeBonusActive = true;
            if (stateGame.bonus.powers[2] == 1) renderingState.isHalfFactoryBonusActive = true;
            if (stateGame.bonus.powers[3] == 1) renderingState.isArmageddonBonusActive = true;
        }
    }
}