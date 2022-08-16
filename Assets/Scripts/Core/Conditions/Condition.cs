using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.Conditions
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] PredicateType predicate;
        [NonReorderable] [SerializeField] ScriptableObject[] parameters;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                bool? result = evaluator.Evaluate(predicate, parameters);
                if (result == null)
                {
                    continue;
                }

                if (result == false) return false;
            }

            return true;
        }
    }
}