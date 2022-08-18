using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Utils
{
    [System.Serializable]
    public class ConjunctionCondition
    {
        [SerializeField]
        Disjunction[] and;

        public bool Check(IEnumerable<IStringPredicateEvaluator> evaluators)
        {
            foreach (Disjunction dis in and)
            {
                if (!dis.Check(evaluators))
                {
                    return false;
                }
            }
            return true;
        }

        [System.Serializable]
        public class Disjunction
        {
            [SerializeField]
            [NonReorderable] Predicate[] or;

            public bool Check(IEnumerable<IStringPredicateEvaluator> evaluators)
            {
                foreach (Predicate pred in or)
                {
                    if (pred.Check(evaluators))
                    {
                        return true;
                    }
                }
                return false;
            }

            public Predicate[] GetOr()
            {
                return or;
            }
        }

        [System.Serializable]
        public class Predicate
        {
            [SerializeField]
            protected EPredicate predicate;
            [SerializeField]
            string[] parameters;
            [SerializeField]
            bool negate = false;

            public bool Check(IEnumerable<IStringPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null)
                    {
                        continue;
                    }

                    if (result == negate) return false;
                }
                return true;
            }

            public string[] GetParameters()
            {
                return parameters;
            }

        }

        public string GetConditionText()
        {
            string text = "";
            foreach (Disjunction dis in and)
            {
                foreach (Predicate pred in dis.GetOr())
                {
                    if (text != "")
                    {
                        text += "\n";
                    }
                    string[] parameters = pred.GetParameters();
                    text += String.Join(" : ", parameters);
                }
            }

            if (text != "")
            {
                text = "Requirements:\n" + text;
            }
            return text;
        } 
    }
}