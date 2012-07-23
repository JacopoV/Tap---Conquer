using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;


namespace TapAndConquer.GameObjects
{
    public class GameObject
    {

        private int radius = 0;
        private float rotation = 0;
        private Vector2 center;
        private Vector2 currentPos;

        private float speed = 0;
        private Vector2 velocity = new Vector2(0,0);
        private float timeLimit = 0;
        private float startTime = 0;

        

        private int objectType;
        private int elements;

        public GameObject(Vector2 position, int owner, int elements)
        {
            this.objectType = owner;
            this.elements = elements;
            currentPos = new Vector2(position.X, position.Y);
        }

        public Vector2 CurrentPos
        {
            set
            {
                currentPos.X = value.X;
                currentPos.Y = value.Y;
            }

            get { return currentPos; }
        }

        virtual public int getTextureNumber()
        {
            return -1;
        }

        public Vector2 Center
        {
            set
            {
                center.X = value.X;
                center.Y = value.Y;
            }

            get { return center; }
        }

        public float Rotation
        {
            set { rotation = value; }

            get { return rotation; }
        }

        public float Speed
        {
            set { speed = value; }

            get { return speed; }
        }

        public Vector2 Velocity
        {
            set
            {
                velocity.X = value.X;
                velocity.Y = value.Y;
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

        public int Elements
        {
            set { elements = value; }

            get { return elements; }
        }

        public int Radius
        {
            set { radius = value; }

            get { return radius; }
        }
    }
}
