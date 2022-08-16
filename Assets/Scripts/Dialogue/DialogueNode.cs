using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameDevTV.Utils;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private bool isPlayerChoice = false;
        [SerializeField] [TextArea(15,20)] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(10, 10, 200, 200);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [NonReorderable] [SerializeField] private List<ConjunctionCondition> conditions = new List<ConjunctionCondition>();

        // Will return true if ALL the conditions are fulfilled.
        public bool CheckCondition(IEnumerable<IStringPredicateEvaluator> evaluators)
        {

            foreach (ConjunctionCondition condition in conditions)
            {
                bool result = condition.Check(evaluators);
                if (!result) return false;
            }

            return true;
        }
        
        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }
        
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public bool IsPlayerChoice()
        {
            return isPlayerChoice;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }
        
        public string GetOnExitAction()
        {
            return onExitAction;
        }


#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
           // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
        }
        
                
        public void SetPlayerIsSpeaking(bool b)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = b;
            // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
        }

        public void SetPlayerChoice(bool b)
        {
            Undo.RecordObject(this, "Change Dialogue Choice");
            isPlayerChoice = b;
            // EditorUtility.SetDirty(this); // Necessary for some Unity versions.
        }
#endif
    }
}