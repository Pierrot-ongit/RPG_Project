using UnityEngine;

namespace RPG.Control
{
    public class ChestPickup : ClickableElement
    {
        
        [SerializeField] GameObject chestLock;
        [SerializeField] Animator chestAnimator;
        [SerializeField] GameObject chestLoot;

        private bool hasBeenOpened = false;
        
        public override void InteractionEffects()
        {
            OpenChest();
        }
        
        public override CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        
        public void OpenChest()
        {
            if (hasBeenOpened) return; // Can only be opened once.

            GetComponent<Collider>().enabled = false;
            if (chestLoot != null)
            {
                chestLoot.gameObject.SetActive(true);
            }
            if (chestLock != null)
            {
                chestLock.SetActive(false);
            }
            if (chestAnimator != null)
            {
                chestAnimator.SetTrigger("open");
            }

            hasBeenOpened = true;
        }
    }
}