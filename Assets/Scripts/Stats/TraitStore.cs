using System;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, ISaveable, IModifierProvider, IStringPredicateEvaluator
    {
        [NonReorderable][SerializeField] private TraitBonus[] bonusConfig;
        [System.Serializable]
        class TraitBonus
        {
            public Trait trait;
            public Stat stat;
            public float additiveBonusPerPoint = 0;
            public float percentageBonusPerPoint = 0;
        }

        private Dictionary<Stat, Dictionary<Trait, float>> additivesBonusCache;
        private Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache;

        private Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
        private Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();
        private BaseStats baseStats;

        public event Action traitsChanged;

        private void Awake()
        {
            additivesBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            foreach (TraitBonus bonus in bonusConfig)
            {
                if (!additivesBonusCache.ContainsKey(bonus.stat))
                {
                    additivesBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }
                if (!percentageBonusCache.ContainsKey(bonus.stat))
                {
                    percentageBonusCache[bonus.stat] = new Dictionary<Trait, float>();
                }

                additivesBonusCache[bonus.stat][bonus.trait] = bonus.additiveBonusPerPoint;
                percentageBonusCache[bonus.stat][bonus.trait] = bonus.percentageBonusPerPoint;
            }
        }

        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }
        
        public int GetPoints(Trait trait)
        {
            return assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;
        }
        
        public int GetStagedPoints(Trait trait)
        {
            return stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;
        }

        public void AssignPoints(Trait trait, int points)
        {
            if (!CanAssignPoints(trait, points)) return;
            stagedPoints[trait] = GetStagedPoints(trait)  + points;
        }

        public bool CanAssignPoints(Trait trait, int points)
        {
            if (GetStagedPoints(trait) + points < 0) return false;
            if (GetUnassignedPoints() < points) return false;
            return true;
        }

        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalProposedPoints();
        }

        private int GetTotalProposedPoints()
        {
            int total = 0;
            foreach (int points in assignedPoints.Values)
            {
                total += points;
            }
            foreach (int points in stagedPoints.Values)
            {
                total += points;
            }

            return total;
        }

        public void Commit()
        {
            foreach (Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }
            stagedPoints.Clear();
            traitsChanged?.Invoke();
        }

        public int GetAssignablePoints()
        {
            if (baseStats == null)
            {
                baseStats = GetComponent<BaseStats>();
            }
            return (int)baseStats.GetStat(Stat.TotalTraitsPoints);
        }
        

        public object CaptureState()
        {
            return assignedPoints;
        }

        public void RestoreState(object state)
        {
            assignedPoints = new Dictionary<Trait, int>(state as IDictionary<Trait, int>);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (!additivesBonusCache.ContainsKey(stat)) yield break;
            foreach (Trait trait in additivesBonusCache[stat].Keys)
            {
                float bonus = additivesBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (!percentageBonusCache.ContainsKey(stat)) yield break;
            foreach (Trait trait in percentageBonusCache[stat].Keys)
            {
                float bonus = percentageBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            
            if (predicate == EPredicate.MinimumTrait)
            {
                if (Enum.TryParse<Trait>(parameters[0], out Trait trait))
                {
                    return GetPoints(trait) >= Int32.Parse(parameters[1]);
                }
                return false;
            }
            return null;
        }
    }
}