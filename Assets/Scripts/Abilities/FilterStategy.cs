using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public abstract class FilterStategy : ScriptableObject
    {
        public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter);
    }
}