using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = "RPG/Abilities/Effects/TriggerAnimationEffect", order = 0)]
    public class TriggerAnimationEffect : EffectStategy
    {
        [SerializeField]string trigger = "";

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().GetComponent<Animator>().SetTrigger(trigger);
            finished();
        }
    }
}