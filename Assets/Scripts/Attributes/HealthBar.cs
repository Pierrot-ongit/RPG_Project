using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        public void UpdateHealthBar(Health healthComponent)
        {
            if (Mathf.Approximately(healthComponent.GetFraction(), 0)
                || Mathf.Approximately(healthComponent.GetFraction(), 1)
               )
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}