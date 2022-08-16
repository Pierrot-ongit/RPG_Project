using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;
using TMPro;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text textHealth;
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                textHealth.gameObject.SetActive(false);
                return;
            }
            Health health = fighter.GetTarget();
            if (health.IsDead())
            {
                textHealth.gameObject.SetActive(false);
                return;
            }
            textHealth.gameObject.SetActive(true);
            textHealth.text = String.Format("{0:0}/{1:0}", health.GetCurrentHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}