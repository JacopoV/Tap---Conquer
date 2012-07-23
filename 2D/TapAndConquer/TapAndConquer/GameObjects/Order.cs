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
    class Order
    {
        private List<Factory> origin = new List<Factory>();
        private List<Factory> target = new List<Factory>();
        

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

        //public List<Troop> execute(int player)
        //{
        //    List<Troop> newTroops = new List<Troop>();
        //    bool create = false;

        //    if (origin.Count > 0 && origin[0].ObjectType == player && origin[0].getSendableTroops(false) > 0)
        //    {
        //        newTroops.Add(new Troop(origin[0].ObjectType, origin[0].Center, origin[0].getSendableTroops(true), target[0]));
        //        create = true;
        //    }

        //    if (origin.Count > 1 && origin[1].ObjectType == player && origin[1].getSendableTroops(false) > 0)
        //    {
        //        newTroops.Add(new Troop(origin[1].ObjectType, origin[1].Center, origin[1].getSendableTroops(true), target[0]));
        //        if (newTroops.Count > 1) 
        //        {
        //            newTroops[0].Fellow = newTroops[1];
        //            newTroops[1].Fellow = newTroops[0];
        //        }
        //        create = true;
        //    }

        //    if (create)
        //        return newTroops;
        //    else
        //        return null;
        //}
    }
}
