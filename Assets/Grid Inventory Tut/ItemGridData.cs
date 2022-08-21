using UnityEngine;

namespace GridInventoryTutorial
{
    [CreateAssetMenu(fileName = "ItemGrid Data", menuName = "ItemGridData", order = 0)]
    public class ItemGridData : ScriptableObject
    {
        public int width = 1;
        public int height = 1;
        public Sprite itemIcon;
    }
}