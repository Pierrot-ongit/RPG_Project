using System;
using GameDevTV.Inventories;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        private Shopper shopper = null;
        private Shop currentShop = null;
        [SerializeField] private TextMeshProUGUI shopName;
        [SerializeField] private TextMeshProUGUI totalField;
        [SerializeField] Transform listRoot;
        [SerializeField] RowUI rowPrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button switchButton;

        private Color originalTotalTextColor;
        
        private void Start()
        {
            originalTotalTextColor = totalField.color;
           shopper = GameObject.FindWithTag("Player").GetComponent<Shopper>();
           if (shopper == null) return;
           shopper.activeShopChange += ShopChange;
           confirmButton.onClick.AddListener(ConfirmTransaction);
           switchButton.onClick.AddListener(SwitchMode);
           ShopChange();
        }

        private void ShopChange()
        {
            if (currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }
            
            currentShop = shopper.GetActiveShop();
            gameObject.SetActive(currentShop != null);

            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
            }
            
            if (currentShop == null) return;
            
            shopName.text = currentShop.GetShopName();
            currentShop.onChange += RefreshUI;
            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (ShopItem shopItem in currentShop.GetFilteredItems())
            {
                RowUI uiInstance = Instantiate<RowUI>(rowPrefab, listRoot);
                uiInstance.Setup(shopItem, currentShop);
            }

            totalField.text = $"Total: ${currentShop.TransactionTotal():N2}";
            totalField.color = currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;
            confirmButton.interactable = currentShop.CanTransact();

            TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode())
            {
                switchText.text = "Switch To Selling";
                confirmText.text = "Buy";
                
            }
            else
            {
                switchText.text = "Switch To Buying";
                confirmText.text = "Sell";
            }
            
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }

        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}