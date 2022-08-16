using System;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;
        BaseStats _baseStats;

        private void Awake()
        {
            mana = new LazyValue<float>(GetMaxMana);
            _baseStats = GetComponent<BaseStats>();
        }

        private void Update()
        {
            float maxMana = GetMaxMana();
            if (mana.value < maxMana)
            {
                mana.value += GetRegenRate() * Time.deltaTime;
                if (mana.value > maxMana)
                {
                    mana.value = maxMana;
                }
            }
            
        }
        
        private float GetInitialMana()
        {
            return _baseStats.GetStat(Stat.Mana);
        }

        public float GetMana()
        {
            return mana.value;
        }
        public float GetMaxMana()
        {
            return _baseStats.GetStat(Stat.Mana);
        }
        public float GetRegenRate()
        {
            return _baseStats.GetStat(Stat.ManaRegenRate);
        }
        

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana.value)
            {
                return false;
            }
            mana.value -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }

        public float GetFraction()
        {
            return mana.value / _baseStats.GetStat(Stat.Mana);
        }
    }
}