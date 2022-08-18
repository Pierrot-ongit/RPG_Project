using System;
using GameDevTV.Inventories;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Inventories.Editor
{
    public class InventoryItemEditor : EditorWindow
    {
        
        [NonSerialized] InventoryItem selected = null;
        GUIStyle previewStyle;
        GUIStyle descriptionStyle;
        GUIStyle headerStyle;
        bool stylesInitialized = false;
        private Font fontPreview;

        void OnEnable()
        {
            previewStyle = new GUIStyle();
            previewStyle.normal.background = EditorGUIUtility.Load("Assets/Asset Packs/Fantasy RPG UI Sample/UI/Parts/Background_06.png") as Texture2D;
            previewStyle.padding = new RectOffset(40, 40, 40, 40);
            previewStyle.border = new RectOffset(0, 0, 0, 0);
            fontPreview = EditorGUIUtility.Load("Assets/TextMesh Pro/Fonts/Courgette-Regular.ttf") as Font;
        }


        [MenuItem("Window/InventoryItem Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(InventoryItemEditor), false, "InventoryItem Editor");
        }
        
        public static void ShowEditorWindow(InventoryItem candidate) { 
            InventoryItemEditor window = GetWindow(typeof(InventoryItemEditor), false, "InventoryItem") as InventoryItemEditor;
            if (candidate)
            {
                window.OnSelectionChange();
            }
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            InventoryItem candidate = EditorUtility.InstanceIDToObject(instanceID) as InventoryItem;
            if (candidate != null)
            {
                ShowEditorWindow(candidate);
                return true;
            }
            return false;
        }
        
        void OnSelectionChange()
        {
            var candidate = EditorUtility.InstanceIDToObject(Selection.activeInstanceID) as InventoryItem;
            if (candidate == null) return;
            selected = candidate;
            Repaint();
        }
        
        void OnGUI()
        {

            if (!selected)
            {
                EditorGUILayout.HelpBox("No Item Selected", MessageType.Error);
                return;
            }

            if (!stylesInitialized)
            {
                descriptionStyle = new GUIStyle(GUI.skin.label)
                {
                    richText = true,
                    wordWrap = true,
                    stretchHeight = true,
                    fontSize = 24,
                    alignment = TextAnchor.MiddleCenter,
                    font = fontPreview
                };
                headerStyle = new GUIStyle(descriptionStyle) { fontSize = 24 };
                stylesInitialized = true;
            }
            Rect rect = new Rect(0, 0, position.width * .65f, position.height);
            EditorGUILayout.BeginHorizontal();
           // GUILayout.FlexibleSpace();
            DrawInspector(rect);
            rect.x = rect.width;
            rect.width /= 2.0f;

            DrawPreviewTooltip(rect);
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.HelpBox($"{selected.name}/{selected.GetDisplayName()}", MessageType.Info);
           // selected.DrawCustomInspector();
            
        }
        
        Vector2 scrollPosition;
        void DrawInspector(Rect rect)
        {
           // GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selected.DrawCustomInspector();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
           // GUILayout.EndArea();
        }
        
        void DrawPreviewTooltip(Rect rect)
        {
           // GUILayout.BeginArea(rect, previewStyle);
            GUILayout.BeginVertical(previewStyle);
            if (selected.GetIcon() != null)
            {
                float iconSize = Mathf.Min(rect.width * .33f, rect.height * .33f);
                Rect texRect = GUILayoutUtility.GetRect(iconSize, iconSize);
                GUI.DrawTexture(texRect, selected.GetIcon().texture, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.LabelField(selected.GetDisplayName(), headerStyle);
            EditorGUILayout.LabelField(selected.GetDescription(), descriptionStyle);
            GUILayout.EndVertical();
          //  GUILayout.EndArea();
        }
        
    }
}