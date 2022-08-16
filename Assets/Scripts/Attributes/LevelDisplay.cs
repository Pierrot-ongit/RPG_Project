using UnityEngine;
using TMPro;
using System;
using RPG.Stats;

namespace RPG.Attributes
{
    public class LevelDisplay : MonoBehaviour
    {
        
        [SerializeField] TMP_Text textXP;
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {

            textXP.text = String.Format("Level : {0:0}", baseStats.GetLevel());
        }
    }
}
