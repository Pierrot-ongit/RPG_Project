using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Self Targeting", menuName = "RPG/Abilities/Targeting/Self Target", order = 0)]
    public class SelfTargeting : TargetingStategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            data.SetTargets( new GameObject[]{data.GetUser()});
            data.SetTargetedPoint(data.GetUser().transform.position);
            finished();
        }
    }
}
