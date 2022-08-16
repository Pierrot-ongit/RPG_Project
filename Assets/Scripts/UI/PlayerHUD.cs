using System;
using RPG.Attributes;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] RectTransform foregroundHealth = null;
        [SerializeField] RectTransform foregroundMana = null;
        [SerializeField] RectTransform foregroundExperience = null;
        [SerializeField] TMP_Text textLevel;
        Health health;
        BaseStats baseStats;
        Mana mana;

        private void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            health = player.GetComponent<Health>();
            mana = player.GetComponent<Mana>();
            baseStats = player.GetComponent<BaseStats>();
        }
        
        private void Update()
        {
          UpdateHealthBar();
          UpdateManahBar();
          UpdateExperienceBar();

        }
        
        public void UpdateHealthBar()
        {
            foregroundHealth.localScale = new Vector3(health.GetFraction(), 1, 1);
        }
        public void UpdateManahBar()
        {
            foregroundMana.localScale = new Vector3(mana.GetFraction(), 1, 1);
        }
        
        public void UpdateExperienceBar()
        {
            foregroundExperience.localScale = new Vector3(baseStats.GetFractionLevel(), 1, 1);
            textLevel.text = String.Format("Level : {0:0}", baseStats.GetLevel());
        }
        
    }
}