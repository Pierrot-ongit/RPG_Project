using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using UnityEditor;
using Object = UnityEngine.Object;

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
        
        public override string GetDescription()
        {
            string result = projectile ? "Ranged Weapon" : "Melee Weapon";
            result += $"\n\n{GetRawDescription()}\n";
            result += $"\nRange {weaponRange} meters";
            result += $"\nBase Damage {weaponDamage} points";
            if ((int)percentageDamageBonus != 0)
            {
                string bonus = percentageDamageBonus > 0 ? "<color=#8888ff>bonus</color>" : "<color=#ff8888>penalty</color>";
                result += $"\n{(int) percentageDamageBonus} percent {bonus} to attack.";
            }
            return result;
        }
        
        
     #if UNITY_EDITOR

        void SetWeaponRange(float newWeaponRange)
        {
            if (FloatEquals(weaponRange, newWeaponRange)) return;
            SetUndo("Set Weapon Range");
            weaponRange = newWeaponRange;
            Dirty();
        }

        void SetWeaponDamage(float newWeaponDamage)
        {
            if (FloatEquals(weaponDamage, newWeaponDamage)) return;
            SetUndo("Set Weapon Damage");
            weaponDamage = newWeaponDamage;
            Dirty();
        }

        void SetPercentageBonus(float newPercentageBonus)
        {
            if (FloatEquals(percentageDamageBonus, newPercentageBonus)) return;
            SetUndo("Set Percentage Bonus");
            percentageDamageBonus = newPercentageBonus;
            Dirty();
        }

        void SetIsRightHanded(bool newRightHanded)
        {
            if (isRightHanded == newRightHanded) return;
            SetUndo(newRightHanded?"Set as Right Handed":"Set as Left Handed");
            isRightHanded = newRightHanded;
            Dirty();
        }

        void SetAnimatorOverride(AnimatorOverrideController newOverride)
        {
            if (newOverride == animatorOverride) return;
            SetUndo("Change AnimatorOverride");
            animatorOverride = newOverride;
            Dirty();
        }

        void SetEquippedPrefab(Weapon newWeapon)
        {
            if (newWeapon == equippedPrefab) return;
            SetUndo("Set Equipped Prefab");
            equippedPrefab = newWeapon;
            Dirty();
        }

        void SetProjectile(Projectile possibleProjectile)
        {
           // if (!possibleProjectile.TryGetComponent<Projectile>(out Projectile newProjectile)) return;
            if (possibleProjectile == projectile) return;
            SetUndo("Set Projectile");
            projectile = possibleProjectile;
            Dirty();
        }
        
        public override bool IsLocationSelectable(Enum location)
        {
            EquipLocation candidate = (EquipLocation)location;
            return candidate == EquipLocation.Weapon;
        }
        
        bool drawWeaponItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            drawWeaponItem = EditorGUILayout.Foldout(drawWeaponItem, "WeaponConfig Data", foldoutStyle);
            if (!drawWeaponItem) return;
            EditorGUILayout.BeginVertical(contentStyle);
            SetEquippedPrefab((Weapon)EditorGUILayout.ObjectField("Equipped Prefab", equippedPrefab,typeof(Object), false));
            SetWeaponDamage(EditorGUILayout.Slider("Weapon Damage", weaponDamage, 0, 100));
            SetWeaponRange(EditorGUILayout.Slider("Weapon Range", weaponRange, 1,40));
            SetPercentageBonus(EditorGUILayout.IntSlider("Percentage Bonus", (int)percentageDamageBonus, -10, 100));
            SetIsRightHanded(EditorGUILayout.Toggle("Is Right Handed", isRightHanded));
            SetAnimatorOverride((AnimatorOverrideController)EditorGUILayout.ObjectField("Animator Override", animatorOverride, typeof(AnimatorOverrideController), false));
            SetProjectile((Projectile)EditorGUILayout.ObjectField("Projectile", projectile, typeof(Projectile), false));
            EditorGUILayout.EndVertical();
        }

#endif  
    }
}