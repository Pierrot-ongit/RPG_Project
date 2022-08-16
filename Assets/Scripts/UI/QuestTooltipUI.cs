using System;
using RPG.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI
{
    public class QuestTooltipUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;
        [SerializeField] private QuestRewardUI rewardPrefab;
        [SerializeField] private Transform rewardsContainer;
        
        public void Setup(QuestStatus questStatus)
        {
            Quest quest = questStatus.GetQuest();
            title.text = quest.GetTitle();
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                if (questStatus.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate<GameObject>(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }
            
            foreach (Transform child in rewardsContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Quest.ItemReward reward in quest.GetRewards())
            {
                QuestRewardUI rewardInstance = Instantiate<QuestRewardUI>(rewardPrefab, rewardsContainer);
                rewardInstance.Setup(reward.item, reward.number);
            }

        }
    }
}