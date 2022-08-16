using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Inventories;
using RPG.Control;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        // Interaction properties.
        private bool hasBeenSelectionned = false;
        [SerializeField] float interactionRange = 3f;
        GameObject player;

        [SerializeField] private string shopName = "Blacksmith Shop";
        [SerializeField] private float sellingPercentage = 50f;
        [SerializeField] private float maxBarterDiscount = 80;
        [NonReorderable][SerializeField] private List<StockItemConfig> stockConfig = new List<StockItemConfig>();
        List<InventoryItem> sellingList = new List<InventoryItem>();

        [System.Serializable]
        class StockItemConfig
        {
            public StockItemConfig(InventoryItem item, int initialStock)
            {
                this.item = item;
                this.initialStock = initialStock;
                buyingDiscountPercentage = 0;
                levelToUnlock = 0;
            }
            
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)]
            public float buyingDiscountPercentage;
            public int levelToUnlock = 0;
        }

        private Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        private Shopper currentShopper = null;
        private bool isBuyingMode = true;
        private ItemCategory filter = ItemCategory.None;

        public event Action onChange;

        private void Awake() {
            foreach (StockItemConfig config in stockConfig)
            {
                stock[config.item] = config.initialStock;
            }
        }

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            sellingList.Clear();
            if (isBuyingMode)
            {
                foreach (StockItemConfig config in stockConfig)
                {
                    float price = GetPrice(config);

                    int quantityInTransaction = 0;
                    transaction.TryGetValue(config.item, out quantityInTransaction);
                    int availibility = GetAvailability(config.item);
                    yield return new ShopItem(config.item, availibility, price, quantityInTransaction);
                }
            }
            else
            {
                Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
                for (int i = 0; i < shopperInventory.GetSize(); i++)
                {
                    InventoryItem item = shopperInventory.GetItemInSlot(i);
                    if (item != null)
                    {
                        int quantityInTransaction = 0;
                        transaction.TryGetValue(item, out quantityInTransaction);
                        float price = 0;
                        if (stock.ContainsKey(item))
                        {
                            foreach (StockItemConfig config in stockConfig)
                            {
                                if (config.item == item)
                                {
                                    price = GetPrice(config);
                                }
                            }
                        }
                        else
                        {
                            price = item.GetPrice() * 0.3f;
                        }

                        if (!sellingList.Contains(item))
                        {
                            sellingList.Add(item);
                            yield return new ShopItem(item, GetAvailability(item), price, quantityInTransaction);
                        }
                    }
                }
            }
        }

        public void SelectFilter(ItemCategory category) {
            filter = category;
            print(category);

            if (onChange != null)
            {
                onChange();
            }
        }

        public ItemCategory GetFilter() 
        { 
            return filter;
        }

        public void SelectMode(bool isBuying) 
        {
            isBuyingMode = isBuying;
            if (onChange != null)
            {
                onChange();
            }
        }

        public bool IsBuyingMode() 
        { 
            return isBuyingMode; 
        }

        public bool CanTransact() 
        { 
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true;

            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) return false;

            return purse.GetBalance() >= TransactionTotal();
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public bool HasInventorySpace()
        {
            if (!isBuyingMode) return true;

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            // Transfer to or from the inventory
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }
            // Removal from transaction
            // Debting or Crediting of funds

            if (onChange != null)
            {
                onChange();
            }
        }

        public float TransactionTotal()
        { 
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }
            return total;
        }

        public string GetShopName()
        {
            return shopName;
        }

        public void AddToTransaction(InventoryItem item, int quantity) 
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }


            int availability = GetAvailability(item);
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }
            
            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            if (onChange != null)
            {
                onChange();
            }
        }

        private int GetAvailability(InventoryItem item)
        {
            if (isBuyingMode)
            {
                return stock[item];
            }

            return CountItemsInInventory(item);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();
            if (inventory == null) return 0;

            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        private float GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return config.item.GetPrice()* GetBarterDiscount() * (1 - config.buyingDiscountPercentage / 100);
            }

            return config.item.GetPrice() * (sellingPercentage / 100);
        }

        private float GetBarterDiscount()
        {
           float discount = currentShopper.GetComponent<BaseStats>().GetStat(Stat.BuyingDiscountPercentage);
           return (1 - Mathf.Min(discount, maxBarterDiscount) / 100);
        }


        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;
            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            if (stock.ContainsKey(item))
            {
                stock[item]++;
            }
            else
            {
                stockConfig.Add(new StockItemConfig(item, 1));
                stock[item] = 1;
            }
            shopperPurse.UpdateBalance(price);
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stock[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }
    

        #region Interactable
        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }
        
        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1);
                hasBeenSelectionned = true;
            }
            return true;
        }
        
        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {

            if (!hasBeenSelectionned) return;

            if (IsCloseEnoughToInteract())
            {
                player.GetComponent<Shopper>().SetActiveShop(this);
                player.GetComponent<Mover>().Cancel();
                hasBeenSelectionned = false;
            }

        }

        private bool IsCloseEnoughToInteract()
        {
            return Vector3.Distance(player.transform.position, transform.position) < interactionRange;
        }

        #endregion

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (var pair in stock)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<InventoryItem, int> tempStock = new Dictionary<InventoryItem, int>(stock);
            stock.Clear();
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            foreach (var pair in saveObject)
            {
                InventoryItem item = InventoryItem.GetFromID(pair.Key);
                stock[item] = pair.Value;
                if (tempStock.ContainsKey(item))
                {
                    tempStock.Remove(item);
                }
            }
            // We add again the new items not present when the game was saved.
            foreach (var pair in tempStock)
            {
                stock[pair.Key] = pair.Value;
            }
        }
    }
}