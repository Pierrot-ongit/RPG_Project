using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "RPG/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [NonReorderable][SerializeField] private List<Objective> objectives = new List<Objective>();
        [NonReorderable][SerializeField] private List<ItemReward> rewards = new List<ItemReward>();
        [SerializeField] private float experienceReward = 0f;
        [SerializeField] private float moneyReward = 0f;
        

        
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
            [Min(1)]
            [SerializeField] int quantity = 1;

            public bool usesCondition = false;
            public ConjunctionCondition completionCondition;

            public int GetQuantity()
            {
                return quantity;
            }
            
        }
        
        [System.Serializable]
        public class ItemReward
        {
            [Min(1)]
            public int number = 1;
            public InventoryItem item;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<ItemReward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveReference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == objectiveReference)
                {
                    return true;
                }
            }

            return false;
        }
        
        public string GetObjectiveDescription(string objectiveReference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == objectiveReference)
                {
                    return objective.description;
                }
            }

            return null;
        }

        public int GetObjectiveQuantityRequired(string objectiveReference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == objectiveReference)
                {
                    return objective.GetQuantity();
                }
            }
            // The objective was not found.
            return -1;
        }

        public float GetExperienceReward()
        {
            return experienceReward;
        }
        
        public float GetMoneyReward()
        {
            return moneyReward;
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }

            return null;
        }

    }
}