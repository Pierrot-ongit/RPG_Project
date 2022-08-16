using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "My Ability", menuName = "RPG/Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private float cooldown;
        [SerializeField] private float manaToUse = 0f;
        [SerializeField] private TargetingStategy targetingStrategy;
        [SerializeField] private FilterStategy[] filterStrategies;
        [SerializeField] private EffectStategy[] effectStrategies;

        public float GetCooldown()
        {
            return cooldown;
        }
        
        public float GetManaToUse()
        {
            return manaToUse;
        }
        
        public override bool Use(GameObject user)
        {
            // Is cooldown still in progress.
            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0)
            {
                return false;
            }
            
            Mana manaUser = user.GetComponent<Mana>();
            if (manaUser.GetMana() < manaToUse)
            {
                return false;
            }

            AbilityData data = new AbilityData(user);
            user.GetComponent<ActionScheduler>().StartAction(data);
            
            targetingStrategy.StartTargeting(data,
                () => {
                 TargetAcquired(data);   
                });
            return true;
        }

        private void TargetAcquired(AbilityData data)
        {
            if (data.IsCancelled()) return;

            Mana manaUser = data.GetUser().GetComponent<Mana>();
            if (!manaUser.UseMana(manaToUse)) return; // Security check if we have correctly used the mana.
            
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldown);
            
            
            foreach (FilterStategy filter in filterStrategies)
            {
                data.SetTargets(filter.Filter(data.GetTargets()));
            }

            foreach (EffectStategy effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {
            
        }
    }
}