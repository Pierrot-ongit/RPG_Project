using System;
using UnityEditor;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    public class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual bool Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
            return false;
        }

        public bool isConsumable()
        {
            return consumable;
        }
        
#if UNITY_EDITOR
        
        void SetIsConsumable(bool value)
        {
            if (consumable == value) return;
            SetUndo(value?"Set Consumable":"Set Not Consumable");
            consumable = value;
            Dirty();
        }

        bool drawActionItem = true;
        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            drawActionItem = EditorGUILayout.Foldout(drawActionItem, "Action Item Data");
            if (!drawActionItem) return;
            EditorGUILayout.BeginVertical(contentStyle);
            SetIsConsumable(EditorGUILayout.Toggle("Is Consumable", consumable));
            EditorGUILayout.EndVertical();
        }

#endif
    }
}