using GameDevTV.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class QuestRewardUI : MonoBehaviour
    {
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemTitle;
        [SerializeField] TextMeshProUGUI itemNumber;

        public void Setup(InventoryItem item, int number)
        {
            itemTitle.text = item.GetDisplayName();
            if (number > 1)
            {
                itemNumber.text = number.ToString();
            }
            else
            {
                itemNumber.gameObject.SetActive(false);
            }

            Sprite sprite = item.GetIcon();
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
            else
            {
                itemIcon.gameObject.SetActive(false);
            }
        }
    }
}