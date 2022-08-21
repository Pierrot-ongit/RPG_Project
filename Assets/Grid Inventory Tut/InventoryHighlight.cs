using UnityEngine;

namespace GridInventoryTutorial
{
    public class InventoryHighlight : MonoBehaviour
    {
        [SerializeField] private RectTransform highlighter;

        public void Show(bool b)
        {
            highlighter.gameObject.SetActive(b);
        }
        
        public void SetParent(InventoryGrid targetGrid)
        {
            if (targetGrid == null) { return; }
            highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        }
        
        public void SetSize(InventoryGridItem targetItem)
        {
            Vector2 size = new Vector2();
            size.x = targetItem.data.width * InventoryGrid.tileSizeWidth;
            size.y = targetItem.data.height * InventoryGrid.tileSizeHeight;
            highlighter.sizeDelta = size;
        }

        public void SetPosition(InventoryGrid targetGrid, InventoryGridItem targetItem)
        {
            Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);
            highlighter.localPosition = pos;
        }


        public void SetPosition(InventoryGrid targetGrid, InventoryGridItem targetItem, int posX, int posY)
        {
            Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
            highlighter.localPosition = pos;
        }
    }
}