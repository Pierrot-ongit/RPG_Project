using UnityEngine;

namespace RPG.Core
{
    public interface IPredicateEvaluator
    {
        bool? Evaluate(PredicateType predicate, ScriptableObject[] parameters);
    }
}