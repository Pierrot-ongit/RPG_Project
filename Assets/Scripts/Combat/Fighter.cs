using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    [RequireComponent(typeof(Animator), typeof(Mover))]
    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [FormerlySerializedAs("defaultWeapon")] [SerializeField] WeaponConfig defaultWeaponConfig = null;
        [SerializeField] float autoAttackRange = 4f;


        Mover mover;
        Animator animator;
        Health target;
        BaseStats baseStats;
        private Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            equipment = GetComponent<Equipment>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }


        void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead())
            {
                target = FindNewTargetInRange();
                if (target == null) return;
            }

            if (!GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehavior();
            }
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform.position);
            if (timeSinceLastAttack > timeBetweenAttack)
            {
                // This will trigger the hit event.
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        // Animation Event
        private void Hit()
        {
            if (target == null) return;
            float damage = baseStats.GetStat(Stat.Damage);
            BaseStats targetBasesStats = target.GetComponent<BaseStats>();
            if (targetBasesStats != null)
            {
                float defence = targetBasesStats.GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if (currentWeapon.value != null)
            {
               currentWeapon.value.OnHit(); 
            }
            
            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponRange();
        }
        
        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);
                if (candidateDistance < bestDistance)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                // hit.collider.gameObject;
                Health health = hit.collider.GetComponent<Health>();
                if (health == null) continue;
                if (health.IsDead()) continue;
                if (health.gameObject == gameObject) continue;
                yield return health;
            }
        }

        private void UpdateWeapon()
        {
            if (!equipment) return;
            var weapon =  equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeaponConfig);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }
        
        private void StopAnimationAttack()
        {
            animator.SetTrigger("stopAttack");
            animator.ResetTrigger("attack");
        }

        #region PUBLIC

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAnimationAttack();
            target = null;
            mover.Cancel();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }

            if (!mover.CanMoveTo(combatTarget.transform.position)
              && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null && !targetToTest.IsDead());
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        public Health GetTarget()
        {
            return target;
        }
        
        public Transform GetHandTransform(bool isRightHand)
        {
            if (isRightHand)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        #endregion


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            if (currentWeapon != null)
            {
                Gizmos.DrawWireSphere(transform.position, currentWeaponConfig.GetWeaponRange());
            }
        }
    }
}
