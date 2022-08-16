using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient To Target Effect", menuName = "RPG/Abilities/Effects/OrientToTargetEffect", order = 0)]
    public class OrientToTargetEffect : EffectStategy
    {
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().transform.LookAt(data.GetTargetedPoint());
            finished();
        }
    }
}