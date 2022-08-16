using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {

        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodesLookup = new Dictionary<string, DialogueNode>();
        [SerializeField] private Vector2 newNodeOffset = new Vector2(250, 0);

        [SerializeField] private ConjunctionCondition condition;

        private void Awake()
        {
            // Necessary because OnValidate is not call on a build.
            PopulateDictionary();
        }

        // OnValidate is not call on a build so, we add to rely on another method.
        private void OnValidate()
        {
            PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            nodesLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodesLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChidren(DialogueNode parentNode)
        {
            foreach (string nodeChildId in parentNode.GetChildren())
            {
                if (nodesLookup.ContainsKey(nodeChildId))
                {
                    yield return nodesLookup[nodeChildId];
                }
            }
        }
        
        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode node in GetAllChidren(parentNode))
            {
                if (node.IsPlayerChoice())
                {
                    yield return node;
                }
            }
        }
        
        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode parentNode)
        {
            foreach (DialogueNode node in GetAllChidren(parentNode))
            {
                if (!node.IsPlayerChoice())
                {
                    yield return node;
                }
            }
        }
        
        
        #if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (AssetDatabase.GetAssetPath(this)!= "")
            {
                Undo.RecordObject(this, "Added Dialogue Node"); 
            }
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
                newNode.SetPosition(parentNode.GetRect().position + newNodeOffset);
                newNode.SetPlayerChoice(!parentNode.IsPlayerChoice());
            }

            return newNode;
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
        #endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
        
            if (nodes.Count == 0)
            {
               DialogueNode newNode = MakeNode(null);
               AddNode(newNode);
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }

        public bool CheckCondition(IEnumerable<IStringPredicateEvaluator> evaluators)
        {
            if (condition == null) return true;
            return condition.Check(evaluators);
        }
    }
}
