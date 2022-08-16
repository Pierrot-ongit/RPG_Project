using System;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable,IItemStore
    {
        [SerializeField] private float startingBalance = 400f;
        private float balance = 0;
        
        public event Action balanceChanged;

        private void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            balanceChanged?.Invoke();
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }

        public int AddItems(InventoryItem item, int number)
        {
            CurrencyItem currency = item as CurrencyItem;
            if (currency == null) return 0;
            UpdateBalance(item.GetPrice() * number);
            return number;
        }
    }
}