using UnityEngine;
using TMPro;
using System;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text textHealth;
        Health health;

        private void Start()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            textHealth.text = String.Format("{0:0}/{1:0}", health.GetCurrentHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
