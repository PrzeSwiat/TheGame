﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGame.Leafs
{
    internal class NettleLeaf : Leaf
    {
        public NettleLeaf(Vector3 worldPosition, string modelFileName, string textureFileName) : base(worldPosition, modelFileName, textureFileName)
        {
        }

        public override void AddToInventory(Player player)
        {
            if (this.ispossible == true)
            {
                player.Inventory.addNettleLeaf();
                this.RemoveFromWorld(); ispossible = false;
            }

        }

        public override void UpdateInventory(Player player)
        {
            if (this.chceckCollison(player))
            {
                //Debug.Write("przecina\n");
                this.AddToInventory(player);
            }
        }
    }
}
