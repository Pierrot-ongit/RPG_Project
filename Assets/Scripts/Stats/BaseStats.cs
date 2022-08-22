using System;
using UnityEngine;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, IStringPredicateEvaluator
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] ProgressionCharacterClass progression = null;
        [SerializeField]bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                onLevelUp?.Invoke();
            }
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        public float GetStat(Stat stat)
        {
            (float addMod, float perMod) = GetModifiers(stat);
            return (GetBaseStat(stat) + addMod) * (1 + perMod / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, GetLevel());
        }

        private (float, float) GetModifiers(Stat stat)
        {
            float addMod = 0.0f;
            float perMod = 0.0f;
            if (!shouldUseModifiers) return (addMod, perMod);

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    addMod += modifier;
                }

                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    perMod += modifier;
                }
            }
            return (addMod, perMod);
        }

        private int CalculateLevel()
        {
            // For enemies, the startingLevel is enought.
            if (experience == null) return startingLevel;

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        public float GetFractionLevel()
        {
            float currentXP = experience.GetPoints();
            int level = CalculateLevel();
            float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, level);
            return currentXP / XPToLevelUp;
        }
        
        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            if (predicate == EPredicate.HasLevel)
            {
                if (int.TryParse(parameters[0], out int testLevel))
                {
                    return currentLevel.value >= testLevel;
                } 
            }
            return null;
        }
    }
}

