using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textBalance;

        private Purse playerPurse = null;

        private void Start()
        {
            playerPurse = GameObject.FindWithTag("Player").GetComponent<Purse>();
            playerPurse.balanceChanged += RefreshUI;
            RefreshUI();
        }

        private void RefreshUI()
        {
            textBalance.text = $"{playerPurse.GetBalance():N2}";
        }
    }
}