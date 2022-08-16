using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Damage Effect", menuName = "RPG/Abilities/Effects/DamageEffect", order = 0)]
    public class DamageEffect : EffectStategy
    {
        [SerializeField][Min(0)] private float damage = 0f;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (GameObject target in data.GetTargets())
            {
                var health = target.GetComponent<Health>();
                if (health)
                {
                    health.TakeDamage(data.GetUser(), damage);
                }
            }

            finished();
        }
    }
}