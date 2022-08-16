using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;
        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool iSChoosing = false;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newConversant , List<Dialogue> dialogues)
        {
            
            foreach (Dialogue dialogue in dialogues)
            {
                if (dialogue.CheckCondition(GetEvaluators()))
                {
                    currentDialogue = dialogue;
                }
            }

            if (currentDialogue == null) return;
            
            currentConversant = newConversant;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            iSChoosing = false;
            currentConversant = null;
            onConversationUpdated?.Invoke();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return iSChoosing;
        }

        public string GetText()
        {
            if (currentNode == null) return "";
            return currentNode.GetText();
        }

        public string GetCurrentSpeaker()
        {
            if (currentNode == null) return "";

            if (currentNode.IsPlayerSpeaking())
            {
                return playerName;
            }
            return currentConversant.GetName();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
           currentNode = chosenNode;
           TriggerEnterAction();
           iSChoosing = false;
           Next();
        }
        

        public void Next()
        {
            int numPlayersResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayersResponses > 0)
            {
                iSChoosing = true;
                TriggerExitAction();
                onConversationUpdated?.Invoke();
                return;
            }
            
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
           // TriggerExitAction(); Will be trigger in Quit instead.
            if (children.Count() > 0)
            {
                int index = Random.Range(0, children.Count());
                currentNode = children[index];
                TriggerEnterAction();
            }
            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChidren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IStringPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IStringPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null && currentNode.GetOnEnterAction() != "")
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }
        private void TriggerExitAction()
        {
            if (currentNode != null && currentNode.GetOnExitAction() != "")
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

    }
}