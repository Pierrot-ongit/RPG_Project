﻿using UnityEngine;
using GameDevTV.Inventories;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.UI.Inventories;

namespace RPG.UI.Inventories
{
    public class InventoryGridSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {

        // STATE
        int index;
        InventoryItem item;
        Inventory inventory;

        // PUBLIC
        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
          //  icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index)); TODO
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }
    }
}