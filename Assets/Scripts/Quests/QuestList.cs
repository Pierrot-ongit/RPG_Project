using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [RequireComponent(typeof(Inventory))]
    public class QuestList : MonoBehaviour, ISaveable, IStringPredicateEvaluator, IPredicateEvaluator
    {
        [NonReorderable] [SerializeField] private List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action onQuestsUpdated;

        public delegate void QuestReceivedDelegate(Quest quest);
        public event QuestReceivedDelegate OnQuestReceived;
        public delegate void QuestCompletedDelegate(Quest quest);
        public event QuestCompletedDelegate OnQuestCompleted;
        public delegate void ObjectiveCompletedDelegate(string objective);
        public event ObjectiveCompletedDelegate OnObjectiveCompleted;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            onQuestsUpdated?.Invoke();
            OnQuestReceived?.Invoke(quest);
        }
        
        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }
        
        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus questStatus = GetQuestStatus(quest);
            if (questStatus == null) return;
            
            bool objectiveCompleted = questStatus.CompleteObjective(objective);
            if (questStatus.IsComplete())
            {
                GiveReward(quest);
                CleanItemsQuest(quest);
                OnQuestCompleted?.Invoke(quest);
            }
            else
            {
                if (objectiveCompleted)
                {
                    OnObjectiveCompleted?.Invoke(quest.GetObjectiveDescription(objective));
                }
            }

            onQuestsUpdated?.Invoke();
        }

        private void CleanItemsQuest(Quest quest)
        {
            Inventory inventory = GetComponent<Inventory>();
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                QuestItem item = inventory.GetItemInSlot(i) as QuestItem;
                if (item != null)
                {
                    if (item.GetItemQuest() == quest)
                    {
                        int number = inventory.GetNumberInSlot(i);
                        inventory.RemoveFromSlot(i, number);
                    }
                }
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                if (!reward.item.IsStackable() && reward.number > 1)
                {
                    for (int i = 0; i < reward.number; i++)
                    {
                        ProcessReward(reward.item, 1);
                    }
                }
                else
                {
                    ProcessReward(reward.item, reward.number);
                }
            }

            Purse purse = GetComponent<Purse>();
            if (purse != null)
            {
               purse.UpdateBalance(quest.GetMoneyReward());
            }
            Experience exp = GetComponent<Experience>();
            if (exp != null)
            {
                exp.GainExperience(quest.GetExperienceReward());
            }
        }

        private void ProcessReward(InventoryItem reward, int number)
        {
            bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward, number);
            if (!success)
            {
                GetComponent<ItemDropper>().DropItem(reward, number);
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            
            return state;
        }


        public void RestoreState(object state)
        {
             List<object> stateList = state as List<object>;
             if (stateList == null) return;
             
             statuses.Clear();
             foreach (object objectState in stateList)
             {
                 statuses.Add(new QuestStatus(objectState));
             }
        }

        public bool? Evaluate(PredicateType predicate, ScriptableObject[] parameters)
        {
            Quest searchQuest = parameters[0] as Quest;
            if (searchQuest == null) return null;
            switch (predicate)
            {
                case PredicateType.HasQuest:
                    return HasQuest(searchQuest);
                case PredicateType.CompletedQuest:
                    return GetQuestStatus(searchQuest).IsComplete();

            }
            return null;
        }
        
        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasQuest:
                    return HasQuest(Quest.GetByName(parameters[0]));
                case EPredicate.CompletedQuest:
                    Quest questToTest = Quest.GetByName(parameters[0]);
                    // Quest did not exist.
                    if (questToTest == null) return null;
                    QuestStatus status = GetQuestStatus(questToTest);
                    // The quest exist but we don't have it yet. So we don't have finished yet.
                    if (status == null) return false;
                    return status.IsComplete();
                case EPredicate.CompletedObjective:
                    QuestStatus teststatus = GetQuestStatus(Quest.GetByName(parameters[0]));
                    if (teststatus==null) return false;
                    return teststatus.IsObjectiveComplete(parameters[1]);
            }
            return null;
        }
    }
}

