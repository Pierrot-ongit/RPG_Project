using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "My Ability", menuName = "RPG/Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private float cooldown;
        [SerializeField] private float manaCost = 0f;
        [SerializeField] private TargetingStategy targetingStrategy;
        [SerializeField] private List<FilterStategy> filterStrategies = new List<FilterStategy>();
        [SerializeField] private List<EffectStategy> effectStrategies = new List<EffectStategy>();

        public float GetCooldown()
        {
            return cooldown;
        }
        
        public float GetManaToUse()
        {
            return manaCost;
        }
        
        public override bool Use(GameObject user)
        {
            // Is cooldown still in progress.
            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0)
            {
                return false;
            }
            
            Mana manaUser = user.GetComponent<Mana>();
            if (manaUser.GetMana() < manaCost)
            {
                return false;
            }

            AbilityData data = new AbilityData(user);
            user.GetComponent<ActionScheduler>().StartAction(data);
            
            targetingStrategy.StartTargeting(data,
                () => {
                 TargetAcquired(data);   
                });
            return true;
        }

        private void TargetAcquired(AbilityData data)
        {
            if (data.IsCancelled()) return;

            Mana manaUser = data.GetUser().GetComponent<Mana>();
            if (!manaUser.UseMana(manaCost)) return; // Security check if we have correctly used the mana.
            
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldown);
            
            
            foreach (FilterStategy filter in filterStrategies)
            {
                data.SetTargets(filter.Filter(data.GetTargets()));
            }

            foreach (EffectStategy effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {
            
        }
        
        public override string GetDescription()
        {
            string result = GetRawDescription()+"\n";
            string spell = isConsumable() ? "potion" : "spell";
            result += $"This {spell} will cost {(int)manaCost} mana ";
            result += $"\nCooldown : {(int)cooldown}  seconds.";
            return result;
        }
        
#if UNITY_EDITOR
        
        void SetCooldown(float value)
        {
            if (cooldown == value) return;
            SetUndo("Set Cooldown");
            cooldown = value;
            Dirty();
        }
        
        void SetManaToUse(float value)
        {
            if (manaCost == value) return;
            SetUndo("Set Mana to use");
            manaCost = value;
            Dirty();
        }
        
        void SetTargetingStrategy(TargetingStategy value)
        {
            if (targetingStrategy == value) return;
            SetUndo("Set Targeting Strategy");
            targetingStrategy = value;
            Dirty();
        }
        
        private void SetValue(List<ScriptableObject> objectsList, int i, ScriptableObject newObject)
        {
            if (objectsList[i] == newObject) return;
            SetUndo("Change XXX Value");
            objectsList[i] = newObject;
            Dirty();
        }
        
        
        void DrawObjectList(List<ScriptableObject> objectsList, string type)
        {
            int elementToDelete = -1;
            for (int i = 0; i < objectsList.Count; i++)
            {
                ScriptableObject modifier = objectsList[i];
                EditorGUILayout.BeginHorizontal();
                SetValue(objectsList, i, (ScriptableObject)EditorGUILayout.ObjectField("FilterStategy", objectsList[i], typeof(ScriptableObject), false));
                if (GUILayout.Button("-"))
                {
                    elementToDelete = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (elementToDelete > -1)
            {
                //RemoveToList(modifierList, elementToDelete);
            }

            if (GUILayout.Button("Add Modifier"))
            {
                
             //   AddToFiltersList(ScriptableObject, objectsList);
            }
        }
        
        

        bool drawAbility = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            drawAbility = EditorGUILayout.Foldout(drawAbility, "Ability Data");
            if (!drawAbility) return;
            EditorGUILayout.BeginVertical(contentStyle);
            SetCooldown(EditorGUILayout.IntSlider("Cooldown (seconds)", (int)cooldown, 0, 300));
            SetManaToUse(EditorGUILayout.IntSlider("Mana Cost", (int)manaCost, 0, 1000));
            SetTargetingStrategy((TargetingStategy)EditorGUILayout.ObjectField("TargetingStategy", targetingStrategy, typeof(TargetingStategy), false));

            EditorGUILayout.EndVertical();
        }

#endif
    }
}