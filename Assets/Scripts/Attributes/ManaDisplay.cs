using UnityEngine;
using TMPro;
using System;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text textMana;
        Mana mana;

        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            textMana.text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}
