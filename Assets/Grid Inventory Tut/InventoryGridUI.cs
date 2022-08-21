using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using GameDevTV.Inventories;
using JetBrains.Annotations;
using UnityEngine.PlayerLoop;

namespace GridInventoryTutorial
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryGridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // CONFIG DATA
        [SerializeField] private RectTransform gridRectTransform;
        [SerializeField] private int gridSizeWidth = 14;
        [SerializeField] private int gridSizeHeight = 8;

        // CACHE
        //InventoryGrid playerInventory;
        InventoryGridController playerInventoryController;
        public const float tileSizeWidth = 32;
        public const float tileSizeHeight = 32;
        private Vector2 positionOnTheGrid = new Vector2();
        private Vector2Int titleGridPosition = new Vector2Int();
        private float paddingX = 16;
        private float paddingY = -16;
        
        
        // STATE
        [CanBeNull] InventoryGridItem[,] slots;

        // LIFECYCLE METHODS

        private void Awake() 
        {
            playerInventoryController = InventoryGridController.GetPlayerInventoryGridController();
        }

        private void Start()
        {
            InitGrid(gridSizeWidth, gridSizeHeight);
        }

        private void InitGrid(int width, int height)
        {
            slots = new InventoryGridItem[width, height];
            Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
            gridRectTransform.sizeDelta = size;
        }



        // PUBLIC
        public Vector2Int GetTileGridPosition(Vector2 mousePosition)
        {
            positionOnTheGrid.x = mousePosition.x - gridRectTransform.position.x;
            positionOnTheGrid.y = gridRectTransform.position.y - mousePosition.y;

            titleGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
            titleGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);
            return titleGridPosition;
        }

        public bool PlaceItem(InventoryGridItem item, int posX, int posY, ref InventoryGridItem overlapItem)
        {
            if (BoundaryCheck(posX, posY, item.data.width, item.data.height) == false) return false;

            if (OverlapCheck(posX, posY, item.data.width, item.data.height, ref overlapItem) == false)
            {
                overlapItem = null;
                return false;
            }

            if (overlapItem != null)
            {
                CleanGridReference(overlapItem);
            }
            
            PlaceItem(item, posX, posY);
            return true;
        }

        public void PlaceItem(InventoryGridItem item, int posX, int posY)
        {
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.SetParent(gridRectTransform);
            for (int x = 0; x < item.data.width; x++)
            {
                for (int y = 0; y < item.data.height; y++)
                {
                    slots[posX + x, posY + y] = item;
                }
            }

            item.onGridPositionX = posX;
            item.onGridPositionY = posY;
            Vector2 position = CalculatePositionOnGrid(item, posX, posY);
            // position.x = posX * tileSizeWidth + paddingX;
            // position.y = -posY * tileSizeHeight + paddingY;
            rectTransform.localPosition = position;
        }

        public Vector2 CalculatePositionOnGrid(InventoryGridItem item, int posX, int posY)
        {
            Vector2 position = new Vector2();
            position.x = (posX * tileSizeWidth + tileSizeWidth * item.data.width / 2);
            position.y = -(posY * tileSizeHeight + tileSizeHeight * item.data.height / 2);
            return position;
        }

        private bool OverlapCheck(int posX, int posY, int dataWidth, int dataHeight, ref InventoryGridItem overlapItem)
        {
            for (int x = 0; x < dataWidth; x++)
            {
                for (int y = 0; y < dataHeight; y++)
                {
                    if (slots[posX + x, posY + y] != null)
                    {
                        if (overlapItem == null)
                        {
                            overlapItem = slots[posX + x, posY + y];
                        }
                        else
                        {
                            if (overlapItem != slots[posX + x, posY + y])
                            {
                                // We have overlap with at least two items.
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
        
        private bool CheckAvailableSpace(int posX, int posY, int dataWidth, int dataHeight)
        {
            for (int x = 0; x < dataWidth; x++)
            {
                for (int y = 0; y < dataHeight; y++)
                {
                    if (slots[posX + x, posY + y] != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            // TODO BUG With draging item.
            //need             GetComponent<CanvasGroup>().blocksRaycasts = false;
            playerInventoryController.SelectedGridInventory = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // TODO BUG With draging item.
            playerInventoryController.SelectedGridInventory = null;
        }
        

        public InventoryGridItem PickupItem(int tilePositionX, int tilePositionY)
        {
            InventoryGridItem toReturn = slots[tilePositionX, tilePositionY];
            if (toReturn == null) return toReturn;
            
            CleanGridReference(toReturn);

            return toReturn;
        }

        private void CleanGridReference(InventoryGridItem item)
        {
            for (int x = 0; x < item.data.width; x++)
            {
                for (int y = 0; y < item.data.height; y++)
                {
                    slots[item.onGridPositionX + x, item.onGridPositionY + y] = null;
                }
            }
        }

        bool PositionCheck(int posX, int posY)
        {
            if (posX < 0 || posY < 0 || posX >= gridSizeWidth || posY >= gridSizeHeight)
            {
                return false;
            }
            return true;
        }

        public bool BoundaryCheck(int posX, int posY, int width, int height)
        {
            if (PositionCheck(posX, posY) == false) {return false;}

            posX += width - 1;
            posY += height - 1;
            if (PositionCheck(posX, posY) == false)  {return false;}

            return true;
        }

        public InventoryGridItem GetItem(int x, int y)
        {
            return slots[x, y];
        }

        public Vector2Int? FindSpaceForObject(InventoryGridItem itemToInsert)
        {
            int height = gridSizeHeight - itemToInsert.data.height + 1;
            int width = gridSizeWidth - itemToInsert.data.width + 1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                { 
                    if (CheckAvailableSpace(x, y, itemToInsert.data.width, itemToInsert.data.height))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return null;
        }
    }
}