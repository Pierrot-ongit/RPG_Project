using RPG.Quests;
using UnityEngine;

namespace RPG.UI
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;
        [SerializeField] Transform rootQuest;
        private QuestList questList;
        
        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            RedrawQuestsUI();
            questList.onQuestsUpdated += RedrawQuestsUI;
        }

        private void RedrawQuestsUI()
        {
            foreach (Transform child in rootQuest)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, rootQuest);
                uiInstance.Setup(status);
            }
        }
    }
}