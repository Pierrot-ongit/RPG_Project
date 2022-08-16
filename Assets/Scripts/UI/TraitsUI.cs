using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI unassignedPointsText;
        [SerializeField] private Button confirmButton;
        
        private TraitStore playerTraits = null;
        
        private void Start()
        {
            playerTraits = GameObject.FindWithTag("Player").GetComponent<TraitStore>();
            confirmButton.onClick.AddListener(() => ConfirmPoints());
        }
        
        private void Update()
        {
            unassignedPointsText.text = playerTraits.GetUnassignedPoints().ToString();
        }

        public void ConfirmPoints()
        {
            playerTraits.Commit();
        }
    }
}