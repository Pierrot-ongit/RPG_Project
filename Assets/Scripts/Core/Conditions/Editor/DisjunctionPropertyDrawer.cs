using System.Collections;
using GameDevTV.Utils;
using UnityEditor;
using UnityEngine;

namespace Core.Conditions.Editor
{
    [CustomPropertyDrawer(typeof(ConjunctionCondition.Disjunction))]
    public class DisjunctionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            GUIStyle styleButtonArrow = CreateButtonStyle("node1");
            GUIStyle styleButtonRemove = CreateButtonStyle("RedTexture", true);
            
            SerializedProperty or = property.FindPropertyRelative("or");
            float propHeight = EditorGUIUtility.singleLineHeight;
            
            Rect upPosition = position;
            upPosition.width -= EditorGUIUtility.labelWidth;
            upPosition.x = position.xMax - upPosition.width;
            upPosition.width /= 3.0f;
            upPosition.height = propHeight;
            Rect downPosition = upPosition;
            downPosition.x += upPosition.width;
            Rect deletePosition = upPosition;
            deletePosition.x = position.xMax - deletePosition.width;
            int itemToRemove = -1;
            int itemToMoveUp = -1;
            int itemToMoveDown = -1;
            
            int idx=0;
            var enumerator = or.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (idx > 0)
                {
                    if (GUI.Button(downPosition, "v", styleButtonArrow)) itemToMoveDown = idx - 1; // Draw Down Button and setting itemToMoveDown
                    EditorGUI.DropShadowLabel(position, "---------------OR---------------", style);
                    position.y += propHeight;
                }
                
                SerializedProperty p = enumerator.Current as SerializedProperty;
                position.height = EditorGUI.GetPropertyHeight(p);
                EditorGUI.PropertyField(position, p);
                position.y += position.height;
                position.height = propHeight;
                
                upPosition.y = deletePosition.y = downPosition.y = position.y; // Sets the y position of our buttons
                if (GUI.Button(deletePosition, "Remove", styleButtonRemove)) itemToRemove = idx; // Draw Remove Button and setting itemToRemove
                if (idx > 0 && GUI.Button(upPosition, "^", styleButtonArrow)) itemToMoveUp = idx; // Draw Up Button and setting itemToMoveUp
                position.y+=propHeight;
                
                idx++;
            }
            
            // Pseudo callbacks for the buttons.
            if (itemToRemove >= 0)
            {
                or.DeleteArrayElementAtIndex(itemToRemove);
            }
            if (itemToMoveUp >= 0)
            {
                or.MoveArrayElement(itemToMoveUp, itemToMoveUp - 1);
            }
            if (itemToMoveDown >= 0)
            {
                or.MoveArrayElement(itemToMoveDown, itemToMoveDown + 1);
            }
            
            
            if (GUI.Button(position, "Add Predicate"))
            {
                or.InsertArrayElementAtIndex(idx);
            }

        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float result=0;
            float propHeight = EditorGUIUtility.singleLineHeight;
            IEnumerator enumerator = property.FindPropertyRelative("or").GetEnumerator();
            bool multiple = false;
            while(enumerator.MoveNext())
            {
                SerializedProperty p = enumerator.Current as SerializedProperty;
                // Since the property is a Predicate, it asks the PredicatePropertyDrawer.GetPropertyHeight().
                result += EditorGUI.GetPropertyHeight(p) + propHeight;
                if (multiple) result += propHeight;
                multiple = true;
            }
            return result + propHeight * 1.5f;
        }
        
        private GUIStyle CreateButtonStyle(string textureName, bool resourcesTexture = false)
        {
            GUIStyle style = new GUIStyle();
            if (resourcesTexture)
            {
                style.normal.background = Resources.Load(textureName) as Texture2D;
            }
            else
            {
                style.normal.background = EditorGUIUtility.Load(textureName) as Texture2D;
            }

            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
    }
}