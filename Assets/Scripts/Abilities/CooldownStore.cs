using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        private Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        private Dictionary<InventoryItem, float> initialCooldownTimers = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            List<InventoryItem> keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach (InventoryItem item in keys)
            {
                cooldownTimers[item] -= Time.deltaTime;
                if (cooldownTimers[item] < 0)
                {
                    cooldownTimers.Remove(item);
                    initialCooldownTimers.Remove(item);
                }
            }
        }

        public void StartCooldown(InventoryItem item, float cooldownTime)
        {
            cooldownTimers[item] = cooldownTime;
            initialCooldownTimers[item] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem item)
        {
            if (cooldownTimers.ContainsKey(item))
            {
                return cooldownTimers[item];
            }

            return 0;
        }

        public float GetFractionRemaining(InventoryItem item)
        {
            
            
            if (cooldownTimers.ContainsKey(item) && initialCooldownTimers.ContainsKey(item))
            {
                return cooldownTimers[item] / initialCooldownTimers[item];
            }

            return 0;
        }
    }
}