using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [ProgressionManageableData]
    [CreateAssetMenu(fileName = "Progression Character Class", menuName = "Stats/New Progression Class", order = 0)]
    public class ProgressionCharacterClass : ScriptableObject
    {
        public CharacterClass characterClass;
        [NonReorderable]
        public ProgressionStat[] stats;

        Dictionary<Stat, float[]> lookupTable = null;

        public float GetStat(Stat stat, int level)
        {
            BuildLookup();

            if (!lookupTable.ContainsKey(stat))
            {
                return 0;
            }

            float[] levels = lookupTable[stat];

            if (levels.Length == 0)
            {
                return 0;
            }

            if (levels.Length < level)
            {
                return levels[levels.Length - 1];
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat)
        {
            BuildLookup();

            float[] levels = lookupTable[stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<Stat, float[]>();
            foreach (ProgressionStat progressionStat in stats)
            {
                lookupTable[progressionStat.stat] = progressionStat.levels;
            }
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;
            [NonReorderable]
            public float[] levels;
        }
    }
}