using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Heal Effect", menuName = "RPG/Abilities/Effects/HealEffect", order = 0)]
    public class HealEffect : EffectStategy
    {
        [SerializeField][Min(0)] private float healPoints = 0f;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (GameObject target in data.GetTargets())
            {
                var health = target.GetComponent<Health>();
                if (health)
                {
                    health.Heal(healPoints);
                }
            }

            finished();
        }
    }
}