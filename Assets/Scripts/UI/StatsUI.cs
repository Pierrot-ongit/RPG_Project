using GameDevTV.Inventories;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class StatsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private TextMeshProUGUI armorText;
        
        private BaseStats baseStats;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            baseStats = player.GetComponent<BaseStats>();
            player.GetComponent<TraitStore>().traitsChanged += RedrawStatsUI;
            player.GetComponent<Equipment>().equipmentUpdated += RedrawStatsUI;
            RedrawStatsUI();
        }

        private void RedrawStatsUI()
        {
            float power = baseStats.GetStat(Stat.Damage);
            powerText.text = power.ToString();
            float defense = baseStats.GetStat(Stat.Defence);
            armorText.text = defense.ToString();
        }
    }
}