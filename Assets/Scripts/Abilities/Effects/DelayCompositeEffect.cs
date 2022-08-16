using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "RPG/Abilities/Effects/DelayComposite", order = 0)]
    public class DelayCompositeEffect : EffectStategy
    {
        [SerializeField] private float delay = 0f;
        [SerializeField] EffectStategy[] delayedEffects;
        [SerializeField] bool abortIfCancelled = false;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
        }
        
        private IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);
            if (abortIfCancelled && data.IsCancelled()) yield break;
            foreach (EffectStategy effect in delayedEffects)
            {
                effect.StartEffect(data, finished);
            }
            finished();
        }
    }
}