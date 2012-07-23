using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace StateGameLayer
{
    public class Troop : GameObject
    {
        public bool fighting;
        public Factory myTarget;
        public Vector3 posOrigin;
        private Troop myFellow = null;
        private Troop enemyTroop = null;
        public bool myFellowArrived = false;
        public bool enemyEngaged = false;
        public int smartEnemyCounter = 0;
        public Vector3 runAbovePlanet = Vector3.Zero;
        public float dimensionPlanetCollision = 0;

        public Troop() {}
        public Troop(int owner, Vector3 position, int elements, Factory targetFactory, int maxHitPoints, int damage, int defense, int shields)
            :base(position, owner, -1, elements, maxHitPoints * elements, damage * elements, defense * elements, shields * elements)
        {
            Rotation = Math.Atan2(targetFactory.CurrentPos.Z - position.Z, targetFactory.CurrentPos.X - position.X);
            myTarget = targetFactory;
            Speed = 600.0f;
            Velocity = new Vector3( (float)Math.Cos(Rotation), 0,(float)Math.Sin(Rotation));
            fighting = false;
            TimeLimit = 0.5f;
            RegenTime = 4;

            //calculate center
            Center = CurrentPos;
            posOrigin = CurrentPos;
        }

        public float DimensionPlanetCollision
        {
            set { dimensionPlanetCollision = value; }

            get { return dimensionPlanetCollision; }
        }
        public Vector3 RunAbovePlanet
        {
            set { runAbovePlanet = value; }

            get { return runAbovePlanet; }
        }

        public void battle() 
        {
            myTarget.attack();
            fighting = true;
        }

        public bool Fighting
        {
            set { fighting = value; }

            get { return fighting; }
        }

        public int SmartEnemyCounter
        {
            set { smartEnemyCounter = value; }

            get { return smartEnemyCounter; }
        }
        
        public Factory Target
        {
            set { myTarget = value; }
            get { return myTarget; }
        }

        public Troop Fellow
        {
            set { myFellow = value; }
            get { return myFellow; }
        }

        public bool FellowArrived
        {
            set { myFellowArrived = value; }
            get { return myFellowArrived; }
        }

        public Troop EnemyTroop
        {
            set { enemyTroop = value; }
            get { return enemyTroop; }
        }

        public bool EnemyEngaged
        {
            set { enemyEngaged = value; }
            get { return enemyEngaged; }
        }

        public override void recalculateElements(int initialHP)
        {
            if (Elements > 0)
            {
                int deads = (int)(((float)(initialHP - HitPoints) / (float)(MaxHitPoints)) * 0.66);
                MaxHitPoints = Math.Max((MaxHitPoints / Elements) * Math.Max(Elements - deads, 0), 1);
                Damage = Math.Max((Damage / Elements) * Math.Max(Elements - deads, 0), 1);
                Defense = Math.Max((Defense / Elements) * Math.Max(Elements - deads, 0), 1);
                MaxShields = Math.Max((MaxShields / Elements) * Math.Max(Elements - deads, 0), 1);
                Elements = Math.Max(Elements - deads, 0);
            }
        }

    };
}
