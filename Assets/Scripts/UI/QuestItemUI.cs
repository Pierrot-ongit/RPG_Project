using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;

        private QuestStatus questStatus;

        public void Setup(QuestStatus newQuestStatus)
        {
            this.questStatus = newQuestStatus;
            title.text = questStatus.GetQuest().GetTitle();
            progress.text = questStatus.GetCompletedObjectives().Count + "/" + questStatus.GetQuest().GetObjectiveCount();
        }

        public QuestStatus GetQuestStatus()
        {
            return questStatus;
        }
    }
}