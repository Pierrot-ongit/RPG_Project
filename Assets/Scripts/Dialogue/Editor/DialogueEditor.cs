using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        private Vector2 scrollPosition;
        
        
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private GUIStyle playerNodeStyle;
        [NonSerialized] GUIStyle selectedNodeStyle; // Style of the selected node
        [NonSerialized] GUIStyle selectedPlayerNodeStyle; // Style of the player selected node
        [NonSerialized] GUIStyle textStyle;
        [NonSerialized]
        private DialogueNode draggingNode = null;
        [NonSerialized]
        private Vector2 draggingOffset;
        [NonSerialized]
        private DialogueNode creatingNode = null;
        [NonSerialized]
        private DialogueNode deletingNode = null;
        [NonSerialized]
        private DialogueNode linkingParentNode = null;
        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;

        string selectedDialogueNodeName = null;     // Name (id) of the selected node

        private const float canvasSize = 4000;
        private const float backgroundSize = 50;
        private const string textureNodeNormal = "node0";
        private const string textureNodeNormalSelected = "node0 on";
        private const string textureNodePlayer = "node1";
        private const string textureNodePlayerSelected = "node1 on";
        
        
        [MenuItem("Window/Dialogue Editor", false, 0)]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // Convert an instanceID to an Object and then try to cast it as Dialogue;
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                DialogueEditor.ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = CreateNodeStyle(textureNodeNormal);
            playerNodeStyle = CreateNodeStyle(textureNodePlayer);

            // Add a new style for the selected node
            selectedNodeStyle = CreateNodeStyle(textureNodeNormalSelected);
            selectedPlayerNodeStyle = CreateNodeStyle(textureNodePlayerSelected);

            textStyle = new GUIStyle();
            textStyle.wordWrap = true;
            textStyle.normal.textColor = Color.white;
            textStyle.stretchHeight = true;

        }

        private GUIStyle CreateNodeStyle(string textureName)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load(textureName) as Texture2D;
            style.normal.textColor = Color.white;
            style.padding = new RectOffset(20, 20, 20, 20);
            style.border = new RectOffset(12, 12, 12, 12);
            return style;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
        

        private void OnSelectionChanged()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue != null)
            {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundText = Resources.Load("background") as Texture2D;
                Rect textCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundText, textCoords);
                
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    DrawConnections(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }
 
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                    // Assign the name of the node when click on it
                    selectedDialogueNodeName = draggingNode.name;
                }
                else
                {
                    draggingCanvas = true;
                    draggingOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                    
                    // Assign null when clicked outside the node
                    selectedDialogueNodeName = null;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.name == selectedDialogueNodeName)
            {
                style = selectedNodeStyle;
            }

            if (node.IsPlayerChoice())
            {
                
                style = playerNodeStyle;
                if (node.name == selectedDialogueNodeName)
                {
                    style = selectedPlayerNodeStyle;
                }
            }

            GUILayout.BeginArea(node.GetRect(), style);

            string newText = EditorGUILayout.TextField(node.GetText(), textStyle);
            node.SetText(newText);


            // Change the color between the GUILayout-horizontal area, so only the buttons are affected
            GUILayout.BeginHorizontal(); 
            Color defaultGUIColor = GUI.backgroundColor; // Saves the default color

            GUI.backgroundColor = Color.red; // Set the color for Delete-button
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            GUI.backgroundColor = Color.blue; // Set the color for the Link-button
            DrawLinkButtons(node);


            GUI.backgroundColor = Color.green; // Set the color for the Add-button
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            
            GUI.backgroundColor = defaultGUIColor; // Sets the default color back
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        
        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }
        
        private void DrawConnections(DialogueNode node)
        {
            if (node.GetChildren() == null) return;
            Vector3 startPosition = new Vector2(node.GetRect().xMax - 5f, node.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChidren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin + 5f, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Color connectionColor = Color.gray;
                if (node.name == selectedDialogueNodeName)
                {
                    connectionColor = Color.white;
                }

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    connectionColor, null, 4f);
            }
        }

        
        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}