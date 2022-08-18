using System.Collections;
using System.Collections.Generic;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// To be placed on icons representing the item in a slot. Allows the item
    /// to be dragged into other slots.
    /// </summary>
    public class InventoryDragItem : DragItem<InventoryItem>, IPointerClickHandler
    {
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // Double click ! We wil try to swap between the equipement and inventory.
            if(eventData.clickCount > 1)
            {
               bool result = HandleEquipableItem();
               if (!result)
               {
                   result = HandleActionItem();
               }
            }
        }

        private bool HandleEquipableItem()
        {
            bool result = false;
            EquipableItem removedSourceItem = source.GetItem() as EquipableItem;
            // If not equipable, don't bother.
            if (removedSourceItem == null) return result;
            IDragContainer<InventoryItem> destination = null;

            if (source is InventorySlotUI)
            {
                // We clicked on an InventorySlot. Lets try to find a corresponding EquipementSlot.
                IEnumerable<EquipmentSlotUI> equipementSlots = FindObjectsOfType<EquipmentSlotUI>();
                foreach (EquipmentSlotUI equipementSlot in equipementSlots)
                {
                    if (equipementSlot.GetEquipLocation() == removedSourceItem.GetAllowedEquipLocation())
                    {
                        destination = equipementSlot;
                        break;
                    }
                }
            }

            if (source is EquipmentSlotUI)
            {
                // We clicked on an EquipementSlot. Lets try to find a empty InventorySlot.
                IEnumerable<InventorySlotUI> inventorySlots = FindObjectsOfType<InventorySlotUI>();

                foreach (InventorySlotUI inventorySlot in inventorySlots)
                {
                    if (inventorySlot.GetItem() == null)
                    {
                        // we find an empty slot.
                        destination = inventorySlot;
                        break;
                    }
                }
            }

            // We find a candidate slot for swaping.
            if (destination != null)
            {
                DropItemIntoContainer(destination);
                result = true;
            }

            return result;
        }
        
        private bool HandleActionItem()
        {
            bool result = false;
            ActionItem removedSourceItem = source.GetItem() as ActionItem;
            // If not equipable, don't bother.
            if (removedSourceItem == null) return result;
            IDragContainer<InventoryItem> destination = null;

            if (source is InventorySlotUI)
            {
                // We clicked on an InventorySlot. Lets try to find an empty ActionSlotUI.
                IEnumerable<ActionSlotUI> actionSlots = FindObjectsOfType<ActionSlotUI>();
                foreach (ActionSlotUI actionSlot in actionSlots)
                {
                    
                    if (actionSlot.GetItem() == null)
                    {
                        destination = actionSlot;
                        break;
                    }
                }
            }

            if (source is ActionSlotUI)
            {
                // We clicked on an ActionSlotUI. Lets try to find a empty InventorySlot.
                IEnumerable<InventorySlotUI> inventorySlots = FindObjectsOfType<InventorySlotUI>();

                foreach (InventorySlotUI inventorySlot in inventorySlots)
                {
                    if (inventorySlot.GetItem() == null)
                    {
                        // we find an empty slot.
                        destination = inventorySlot;
                        break;
                    }
                }
            }

            // We find a candidate slot for swaping.
            if (destination != null)
            {
                DropItemIntoContainer(destination);
                result = true;
            }

            return result;
        }
    }
}