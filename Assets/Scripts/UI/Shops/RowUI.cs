using GameDevTV.Inventories;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image iconField;
        [SerializeField] private TextMeshProUGUI rowName;
        [SerializeField] private TextMeshProUGUI rowAvaibility;
        [SerializeField] private TextMeshProUGUI rowPrice;
        [SerializeField] private TextMeshProUGUI rowQuantity;

        private Shop currentShop;
        private ShopItem item;
        
        public void Setup(ShopItem shopItem, Shop currentShop)
        {
            this.currentShop = currentShop;
            this.item = shopItem;
            iconField.sprite = shopItem.GetIcon();
            rowName.text = shopItem.GetName();
            rowAvaibility.text = $"{shopItem.GetAvailability()}";
            rowPrice.text = $"{shopItem.GetPrice():N2}";
            rowQuantity.text = $"{shopItem.GetQuantityInTransaction()}";
        }

        public void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);

        }
    }
}