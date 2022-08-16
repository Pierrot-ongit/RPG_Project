using System;
using UnityEngine;

namespace RPG.Quests
{
    // TODO Rebuild as QuestHandler to receive and finish quest to.
    public class QuestGiver : MonoBehaviour
    {
        // TODO Rebuild as a List.
        [SerializeField] private Quest quest;
        
        public event Action questGiven;

        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(quest);
            questGiven ?.Invoke();
        }

        public Quest GetQuestGiven()
        {
            return quest;
        }

    }
}