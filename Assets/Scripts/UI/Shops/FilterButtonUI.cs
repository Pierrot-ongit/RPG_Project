using System;
using GameDevTV.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    [RequireComponent(typeof(Button))]
    public class FilterButtonUI : MonoBehaviour
    {
        private Button button;
        private Shop currentShop;
        [SerializeField] private ItemCategory category = ItemCategory.None;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;
        }

        private void SelectFilter()
        {
            currentShop.SelectFilter(category);
        }

        public ItemCategory GetCategory()
        {
            return category;
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != category;
        }
    }
}