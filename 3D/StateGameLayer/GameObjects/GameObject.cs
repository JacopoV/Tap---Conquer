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
    public class GameObject
    {
        public float radius = 0;
        public double rotation = 0;
        public Vector3 center;
        public Vector3 currentPos;

        public float speed = 0;
        public Vector3 velocity = new Vector3(0,0,0);
        public float timeLimit = 0;
        public float startTime = 0;
        
        public int objectType;
        public int area = -1;
        public int elements = 0;
        public int maxHitPoints = 1;       //punti vita massimi
        public int hitPoints = 1;          //quando arrivano a 0 --> morte/conquista dell'oggetto
        public int damage = 0;             //danno che effettua alle altre unita
        public int defense = 0;            //valore da scalare all'attacco prima di subire danni agli scudi/hitPoints
        public int maxShields = 0;         //scudi massimi
        public int shields = 0;            //scalati al posto degli hitPoints fino a 0, si rigenerano col tempo
        public float regenTime = 0;          //tempo prima del quale gli scudi vengono rigenerati


        public GameObject() { }
        public GameObject(Vector3 position, int owner, int elements)
        {
            this.objectType = owner;
            this.elements = elements;
            currentPos = new Vector3(position.X, position.Y, position.Z);
        }

        public GameObject(Vector3 position, int owner, int area, int elements, int maxHitPoints, int damage, int defense, int shields)
        {
            this.objectType = owner;
            this.area = area;
            this.elements = elements;
            this.maxHitPoints = maxHitPoints;
            this.hitPoints = maxHitPoints;
            this.damage = damage;
            this.defense = defense;
            this.shields = shields;
            this.shields = maxShields;
            currentPos = new Vector3(position.X, position.Y, position.Z);
        }

        public float Radius
        {
            set { radius = value; }

            get { return radius; }
        }
       
        public double Rotation
        {
            set { rotation = value; }

            get { return rotation; }
        }

        public Vector3 Center
        {
            set { center = value; }

            get { return center; }
        }

        
        public Vector3 CurrentPos
        {
            set
            {
                currentPos.X = value.X;
                currentPos.Y = value.Y;
                currentPos.Z = value.Z;
            }

            get { return currentPos; }
        }

        public float Speed
        {
            set { speed = value; }

            get { return speed; }
        }

        public Vector3 Velocity
        {
            set
            {
                velocity.X = value.X;
                velocity.Y = value.Y;
                velocity.Z = value.Z;
            }

            get { return velocity; }
        }

        public float TimeLimit
        {
            set { timeLimit = value; }

            get { return timeLimit; }
        }

        public float StartTime
        {
            set { startTime = value; }

            get { return startTime; }
        }

        public int ObjectType
        {
            set { objectType = value; }

            get { return objectType; }
        }

        public int Area
        {
            set { area = value; }

            get { return area; }
        }

        public int Elements
        {
            set { elements = value; }

            get { return elements; }
        }

        public int MaxHitPoints
        {
            set { maxHitPoints = value; }

            get { return maxHitPoints; }
        }

        public int HitPoints
        {
            set { hitPoints = value; }

            get { return hitPoints; }
        }

        public int Damage
        {
            set { damage = value; }

            get { return damage; }
        }

        public int Defense
        {
            set { defense = value; }

            get { return defense; }
        }

        public int MaxShields
        {
            set { maxShields = value; }

            get { return maxShields; }
        }

        public int Shields
        {
            set { shields = value; }

            get { return shields; }
        }

        public float RegenTime
        {
            set { regenTime = value; }

            get { return regenTime; }
        }

        virtual public int getModelNumber()
        {
            return -1;
        }

        virtual public bool isDead()
        {
            return (hitPoints <= 0);
        }

        virtual public void recalculateElements(int initialHP)
        {
        }
    }
}