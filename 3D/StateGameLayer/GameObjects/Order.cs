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
    public class Order
    {
        public List<Factory> origin = new List<Factory>();
        public List<Factory> target = new List<Factory>();

        public Order() {}
        public Order(List<Factory> newOrder)
        {
            for (int i = 0; i < newOrder.Count; ++i)
            {
                if (newOrder[i].ObjectType == newOrder[0].ObjectType && i < (newOrder.Count - 1))
                    origin.Add(newOrder[i]);
                else
                    target.Add(newOrder[i]);
            }
        }

        public List<Factory> Origin
        {
            set { origin = value; }

            get { return origin; }
        }

        public List<Factory> Target
        {
            set { target = value; }

            get { return target; }
        }
    }
}
