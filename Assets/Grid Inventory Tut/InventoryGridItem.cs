using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GridInventoryTutorial
{
    public class InventoryGridItem : MonoBehaviour
    {
        public ItemGridData data;
        [Tooltip("The starter point X of the object")]
        public int onGridPositionX;
        [Tooltip("The starter point Y of the object")]
        public int onGridPositionY;

        public void Setup(ItemGridData itemData)
        {
            data = itemData;
            GetComponent<Image>().sprite = itemData.itemIcon;
            Vector2 size = new Vector2();
            size.x = itemData.width * InventoryGridUI.tileSizeWidth;
            size.y = itemData.height * InventoryGridUI.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;

        }
    }
    
}
