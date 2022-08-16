using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "RPG/Abilities/Filters/TagFilter", order = 0)]
    public class TagFilter : FilterStategy
    {
        [SerializeField] private string tagToFilter;

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach (GameObject gameObject in objectsToFilter)
            {
                if (gameObject.CompareTag(tagToFilter))
                {
                    yield return gameObject;
                }
            }
        }
    }
}