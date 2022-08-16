using UnityEngine;
using TMPro;
using System;

namespace RPG.Attributes
{
    public class ExperienceDisplay : MonoBehaviour
    {
        
        [SerializeField] TMP_Text textXP;
        Experience exp;

        private void Awake()
        {
            exp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {

            textXP.text = String.Format("XP : {0:0}", exp.GetPoints());
        }
    }
}
