using System;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] private Trait trait;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button plusButton;

        private TraitStore playerTraits = null;

        private void Start()
        {
            playerTraits = GameObject.FindWithTag("Player").GetComponent<TraitStore>();
            minusButton.onClick.AddListener(() => Allocate(-1));
            plusButton.onClick.AddListener(() => Allocate(+1));
        }

        private void Update()
        {
            minusButton.interactable = playerTraits.CanAssignPoints(trait, -1);
            plusButton.interactable = playerTraits.CanAssignPoints(trait, +1);
            valueText.text = playerTraits.GetProposedPoints(trait).ToString();
        }

        public void Allocate(int points)
        {
            playerTraits.AssignPoints(trait, points);
        }
        
        
    }
}