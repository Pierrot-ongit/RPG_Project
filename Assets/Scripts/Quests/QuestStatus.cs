using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus
    {
        [SerializeField] private Quest quest;
        [SerializeField] private List<string> completedObjectives = new List<string>();
        private Dictionary<string, int> objectivesOnGoing = new Dictionary<string, int>();

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
            public Dictionary<string, int> objectivesOnGoing;
        }
        
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
            objectivesOnGoing = new Dictionary<string, int>();
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            if (state == null) return;
            this.quest = Quest.GetByName(state.questName);
            this.completedObjectives = state.completedObjectives;
            this.objectivesOnGoing = state.objectivesOnGoing;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public List<string> GetCompletedObjectives()
        {
            return completedObjectives;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public bool CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {
                int objectiveQuantityDone = 1;
                if (this.objectivesOnGoing == null)
                {
                    this.objectivesOnGoing = new Dictionary<string, int>();
                }
                if (objectivesOnGoing.ContainsKey(objective))
                {
                    objectiveQuantityDone += objectivesOnGoing[objective];
                }

                if (objectiveQuantityDone >= quest.GetObjectiveQuantityRequired(objective))
                {
                    completedObjectives.Add(objective);
                    if (objectivesOnGoing.ContainsKey(objective))
                    {
                       objectivesOnGoing.Remove(objective);
                    }
                }
                else
                {
                    objectivesOnGoing[objective] = objectiveQuantityDone;
                }

                return true;
            }

            return false;
        }

        public object CaptureState()
        {
            QuestStatusRecord record = new QuestStatusRecord();
            record.questName = this.quest.GetTitle();
            record.completedObjectives = this.completedObjectives;
            if (this.objectivesOnGoing == null)
            {
                this.objectivesOnGoing = new Dictionary<string, int>();
            }
            record.objectivesOnGoing = this.objectivesOnGoing;
            return record;
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }

            return true;
        }
    }
}