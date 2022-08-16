using GameDevTV.Core.UI.Tooltips;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        [SerializeField] private float offSetSpawn;
        
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus questStatus = GetComponent<QuestItemUI>().GetQuestStatus();
            tooltip.GetComponent<QuestTooltipUI>().Setup(questStatus);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}