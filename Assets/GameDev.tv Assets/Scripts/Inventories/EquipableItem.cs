using System;
using GameDevTV.Utils;
using UnityEditor;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;

        [SerializeField] private ConjunctionCondition equipCondition;

        // PUBLIC
        public bool CanEquip(EquipLocation equipLocation, Equipment equipement)
        {
            if (equipLocation != allowedEquipLocation) return false;
            return equipCondition.Check(equipement.GetComponents<IStringPredicateEvaluator>());
        }

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

        public string GetConditionText()
        {
            return equipCondition.GetConditionText();
        }
        
#if UNITY_EDITOR
        public void SetAllowedEquipLocation(EquipLocation newLocation)
        {
            if (allowedEquipLocation == newLocation) return;
            SetUndo("Change Equip Location");
            allowedEquipLocation = newLocation;
            Dirty();
        }
        
        public virtual bool IsLocationSelectable(Enum location)
        {
            EquipLocation candidate = (EquipLocation)location;
            return candidate != EquipLocation.Weapon;
        }

        bool drawEquipableItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            drawEquipableItem = EditorGUILayout.Foldout(drawEquipableItem, "EquipableItem Data", foldoutStyle);
            if (!drawEquipableItem) return;
            EditorGUILayout.BeginVertical(contentStyle);
            SetAllowedEquipLocation((EquipLocation)EditorGUILayout.EnumPopup(new GUIContent("Equip Location"), allowedEquipLocation, IsLocationSelectable, false));
            EditorGUILayout.EndVertical();
        }
#endif

    }
}