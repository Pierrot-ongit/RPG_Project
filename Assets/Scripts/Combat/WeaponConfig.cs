using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageDamageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            
            Weapon weaponInstance = null;
            if (equippedPrefab != null)
            {
                Transform handTransform = GetHandTansform(rightHand, leftHand);
                weaponInstance = Instantiate(equippedPrefab, handTransform);
                weaponInstance.gameObject.name = weaponName;
            }

            // If default runtimeAnimatorController (character) return null. 
            // But if already override (already equip a weapon), return AnimatorOverrideController.
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(overrideController != null)
            {
               // We return to the default runtimeAnimatorController (character).
               animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weaponInstance;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTansform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform = rightHand;
            if (!isRightHanded) handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTansform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator);
            projectileInstance.SetDamage(calculatedDamage);
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonusDamage()
        {
            return percentageDamageBonus;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageDamageBonus;
            }
        }
    }
}