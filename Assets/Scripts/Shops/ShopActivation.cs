using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopActivation : MonoBehaviour, ISaveable
    {
        [SerializeField] private GameObject iconShop;
        [SerializeField] private Shop shop;
        private bool hasBeenActivated = false;

        public void ActivateShop()
        {
            shop.enabled = true;
            iconShop.SetActive(true);
            hasBeenActivated = true;
        }

        public object CaptureState()
        {
            return hasBeenActivated;
        }

        public void RestoreState(object state)
        {
            hasBeenActivated =  (bool)state;
            if (hasBeenActivated)
            {
                ActivateShop();
            }
        }
    }
}