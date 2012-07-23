using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace TapAndConquer
{
    class Order : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private List<Factory> origin = new List<Factory>();
        private List<Factory> target = new List<Factory>();
        

        public Order(Game game, List<Factory> newOrder)
            : base(game)
        {
            for (int i = 0; i < newOrder.Count; ++i)
            {
                if (newOrder[i].getType() == newOrder[0].getType() && i < (newOrder.Count - 1))
                    origin.Add(newOrder[i]);
                else
                    target.Add(newOrder[i]);
            }
        }

        public List<Troop> execute(int player)
        {
            List<Troop> newTroops = new List<Troop>();
            bool create = false;

            if (origin.Count > 0 && origin[0].getType() == player && origin[0].getSendableTroops(false) > 0)
            {
                newTroops.Add(new Troop(base.Game, origin[0].getType(), origin[0].Center, origin[0].getSendableTroops(true), target[0].Center, target[0]));
                create = true;
            }

            if (origin.Count > 1 && origin[1].getType() == player && origin[1].getSendableTroops(false) > 0)
            {
                newTroops.Add(new Troop(base.Game, origin[1].getType(), origin[1].Center, origin[1].getSendableTroops(true), target[0].Center, target[0]));
                if (newTroops.Count > 1) 
                {
                    newTroops[0].setFellow(newTroops[1]);
                    newTroops[1].setFellow(newTroops[0]);
                }
                create = true;
            }

            if (create)
                return newTroops;
            else
                return null;
        }
    }
}
