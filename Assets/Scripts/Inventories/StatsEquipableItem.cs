using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using RPG.Core;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/StatsEquipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider, IConditionSearchable
    {
        [NonReorderable] [SerializeField] private Modifier[] additiveModifiers;
        [NonReorderable] [SerializeField] private Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var modifier in additiveModifiers)
            {
                if (modifier.stat != stat) continue;
                yield return modifier.value;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in percentageModifiers)
            {
                if (modifier.stat != stat) continue;
                yield return modifier.value;
            }
        }

        public string GetStatsModifiersText()
        {
            string text = "";
            foreach (var modifier in additiveModifiers)
            {
                text += modifier.stat.ToString() + " : + " + modifier.value + "\n";
            }
            foreach (var modifier in additiveModifiers)
            {
                text += modifier.stat.ToString() + " : *" + modifier.value + "%\n";
            }
            
            return text;
        }
    }
}