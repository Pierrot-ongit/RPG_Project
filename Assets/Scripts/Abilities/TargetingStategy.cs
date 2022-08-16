using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public abstract class TargetingStategy : ScriptableObject
    {
        public abstract void StartTargeting(AbilityData data, Action finished);
    }
}