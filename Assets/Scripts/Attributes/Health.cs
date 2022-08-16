using System;
using UnityEngine;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        private BaseStats _baseStats;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<string>
        {
        }
        
        LazyValue<float> healthPoints;
        bool wasDeadLastFrame = false;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            _baseStats = GetComponent<BaseStats>();
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            _baseStats.onLevelUp += RegenFullHealth;
        }

        private void OnDisable()
        {
            _baseStats.onLevelUp -= RegenFullHealth;
        }

        public bool IsDead()
        {
            return healthPoints.value <= 0;
        }

        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        public float GetCurrentHealthPoints()
        {
            return healthPoints.value;
        }


        public bool IsFullHealth()
        {
            return (GetMaxHealthPoints() - GetCurrentHealthPoints()) < 1;
        }
        
        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
            UpdateState();
        }
        
        public void RegenHealth(float regenerationPercentage)
        {
            float regenHealtPoints = _baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealtPoints);
            UpdateState();
        }
        public void RegenFullHealth()
        {
            RegenHealth(100);
        }


        public float GetPercentage()
        {
            return 100 * GetFraction();
        }
        
        public float GetFraction()
        {
            return healthPoints.value / _baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(Math.Round(damage).ToString());

            if (IsDead())
            {
                onDie?.Invoke();
                AwardExperience(instigator);
            }
            else
            {
                // TODO Conserver pour l'instant. Barre de santé ne disparait pas a la mort sinon.
                //takeDamage.Invoke(Math.Round(damage).ToString());
            }

            UpdateState();
        }

        private void UpdateState()
        {
            Animator animator = GetComponent<Animator>();
            ActionScheduler actionScheduler = GetComponent<ActionScheduler>();
            // Death just happen.
            if (!wasDeadLastFrame && IsDead())
            {
                actionScheduler.CancelCurrentAction();
                animator.SetTrigger("die");
            }
            // Resurection. Reset the animator.
            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            wasDeadLastFrame = IsDead();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(_baseStats.GetStat(Stat.ExperienceReward));
        }


        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            UpdateState();
        }
    }
}