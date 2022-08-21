
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridInventoryTutorial
{
    public class InventoryGridController : MonoBehaviour
    {
        private InventoryGrid _selectedGridInventory;
        public InventoryGrid SelectedGridInventory
        {
            get => _selectedGridInventory;
            set
            {
                _selectedGridInventory = value;
                _inventoryHighlight.SetParent(value);
            }
        }

        private InventoryGridItem selectedItem;
        private InventoryGridItem overlapItem;
        private RectTransform _rectTransform;

        [SerializeField] private List<ItemGridData> items;
        [SerializeField] private InventoryGridItem prefabGridItem;
        [SerializeField] private Transform canvasTransform;

        private InventoryHighlight _inventoryHighlight;

        private void Awake()
        {
            _inventoryHighlight = GetComponent<InventoryHighlight>();
        }

        public static InventoryGridController GetPlayerInventoryGridController()
        {
            return FindObjectOfType<InventoryGridController>();
        }
        
        private void Update()
        {
            ItemIconDrag();
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CreateRandomItem();
            }
            

            if (SelectedGridInventory == null)
            {
                _inventoryHighlight.Show(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                InsertRandomItem();
            }
            
            HandleHighlight();
            if (Input.GetMouseButtonDown(0))
            {
                LeftMousonButtonPressed();
            }

        }

        private void LeftMousonButtonPressed()
        {
            Vector2Int tilePosition = GetTileGridPosition();
            if (selectedItem == null)
            {
                PickupItem(tilePosition);
            }
            else
            {
                PlaceItem(tilePosition);
            }
        }

        private void PlaceItem(Vector2Int tilePosition)
        {
            bool result = SelectedGridInventory.PlaceItem(selectedItem, tilePosition.x, tilePosition.y, ref overlapItem);
            if (result)
            {
                selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = true;
                selectedItem = null;
                if (overlapItem != null)
                {
                    selectedItem = overlapItem;
                    overlapItem = null;
                   _rectTransform = selectedItem.GetComponent<RectTransform>();
                   selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
        }

        private void PickupItem(Vector2Int tilePosition)
        {
            selectedItem = SelectedGridInventory.PickupItem(tilePosition.x, tilePosition.y);
            if (selectedItem != null)
            {
                _rectTransform = selectedItem.GetComponent<RectTransform>();
                selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;
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

        private Vector2 oldPosition;
        private InventoryGridItem itemToHighlight;
        private void HandleHighlight()
        {
            Vector2Int positionOnGrid = GetTileGridPosition();
            if (oldPosition == positionOnGrid) { return; }

            oldPosition = positionOnGrid;
            if (selectedItem == null)
            {
                itemToHighlight = SelectedGridInventory.GetItem(positionOnGrid.x, positionOnGrid.y);
                // We are above an item not yet picked.
                if (itemToHighlight != null)
                {
                    _inventoryHighlight.Show(true);
                    _inventoryHighlight.SetParent(SelectedGridInventory);
                    _inventoryHighlight.SetSize(itemToHighlight);
                    _inventoryHighlight.SetPosition(SelectedGridInventory, itemToHighlight);
                }
                else
                {
                    _inventoryHighlight.Show(false);
                }
            }
            else
            {
                // We have a selected item, and will highligh were it land.
                _inventoryHighlight.Show(
                    SelectedGridInventory.BoundaryCheck(
                        positionOnGrid.x, positionOnGrid.y, selectedItem.data.width, selectedItem.data.height
                        ));
                _inventoryHighlight.SetParent(SelectedGridInventory);
                _inventoryHighlight.SetSize(selectedItem);
                _inventoryHighlight.SetPosition(SelectedGridInventory, selectedItem, positionOnGrid.x, positionOnGrid.y);
            }
            
        }
        
        private Vector2Int GetTileGridPosition()
        {
            Vector2 position = Input.mousePosition;
            // We need to take into account the size of items to give an offset on the mouse position.
            if (selectedItem != null)
            {
                position.x -= (selectedItem.data.width - 1) * InventoryGrid.tileSizeWidth / 2;
                position.y += (selectedItem.data.height - 1) * InventoryGrid.tileSizeHeight / 2;
            }

            Vector2Int tilePosition = SelectedGridInventory.GetTileGridPosition(position);
            return tilePosition;
        }
        
        private void CreateRandomItem()
        {
            if (selectedItem != null)
            {
                Destroy(selectedItem);
            }
            
            InventoryGridItem newItem = Instantiate(prefabGridItem, canvasTransform);
            selectedItem = newItem;
            _rectTransform = newItem.GetComponent<RectTransform>();
            _rectTransform.SetParent(canvasTransform);
            int selectedItemID = Random.Range(0, items.Count);
            newItem.Setup(items[selectedItemID]);
            selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        private void InsertRandomItem()
        {
            if (_selectedGridInventory == null) return;
            CreateRandomItem();
            InventoryGridItem itemToInsert = selectedItem;
            selectedItem = null;
            InsertItem(itemToInsert);
        }

        private void InsertItem(InventoryGridItem itemToInsert)
        {
            
            Vector2Int? posOnGrid = _selectedGridInventory.FindSpaceForObject(itemToInsert);
            if (posOnGrid == null)
            {
                // No spave aivailable.
                return;
            }

            _selectedGridInventory.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }
    }
}