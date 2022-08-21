using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryGridUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventoryGridSlotUI _inventoryGridSlotUIPrefab = null;
        [SerializeField] InventoryDragItem _inventoryItemPrefab = null;
        [SerializeField] private List<InventoryItem> items;
        [SerializeField] private Transform canvasTransform;

        // CACHE
        Inventory playerInventory;
        private InventoryDragItem selectedItem;
        private RectTransform _rectTransform;

        // LIFECYCLE METHODS

        private void Awake() 
        {
            playerInventory = Inventory.GetPlayerInventory();
           // playerInventory.inventoryUpdated += Redraw;
        }
        
        private void Update()
        {
            ItemIconDrag();
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CreateRandomItem();
            }

        }
        
        // Visualize Item Drag.
        private void ItemIconDrag()
        {
            if (selectedItem != null)
            {
                _rectTransform.position = Input.mousePosition;
            }
        }
        
        private void CreateRandomItem()
        {
            if (selectedItem != null)
            {
                Destroy(selectedItem);
            }
            
            InventoryDragItem newItem = Instantiate(_inventoryItemPrefab, canvasTransform);
            selectedItem = newItem;
            _rectTransform = newItem.GetComponent<RectTransform>();
            _rectTransform.SetParent(canvasTransform);
            int selectedItemID = Random.Range(0, items.Count);
          //  newItem.Setup(items[selectedItemID]);
            selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
        

        /// <summary>
        /// OLD Functions.
        /// </summary>
        private void Start()
        {
         //   Redraw();
        }

        // PRIVATE

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(_inventoryGridSlotUIPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }
    }
}