using RPG.Quests;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    
    public class NPCQuestIconUI : MonoBehaviour
    {
        [SerializeField] private Image newQuestImage = null;
        [SerializeField] private Image finishQuestImage = null;
        private Quest quest;
        private void Start()
        {
            QuestGiver questGiver = GetComponent<QuestGiver>();
            newQuestImage.gameObject.SetActive(true);
            questGiver.questGiven += QuestHasBeenGiven;
            quest = questGiver.GetQuestGiven();
        }

        private void QuestHasBeenGiven()
        {
            newQuestImage.gameObject.SetActive(false);
        }
        
        private void QuestCanBeFinish()
        {
            finishQuestImage.gameObject.SetActive(true);
        }
        
        private void QuestFinish()
        {
            finishQuestImage.gameObject.SetActive(false);
        }
    }
}