using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public abstract class EffectStategy : ScriptableObject
    {
        public abstract void StartEffect(AbilityData data, Action finished);
    }
}