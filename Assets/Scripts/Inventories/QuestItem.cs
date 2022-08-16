using GameDevTV.Inventories;
using RPG.Quests;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "Quest Item", menuName = ("RPG/Items/Quest Item"))]
    public class QuestItem : InventoryItem
    {
        [SerializeField] Quest quest;

        public Quest GetItemQuest()
        {
            return quest;
        }
        
    }
}