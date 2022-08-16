using System.Collections.Generic;
using System.Linq;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [NonReorderable][SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
       // [SerializeField] private Dialogue AIDialogue = null;
        [SerializeField] string conversantName;
        
        private bool hasBeenSelectionned = false;


        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogues.Count() < 0) return false;
            Health health = GetComponent<Health>();
            if (health != null && health.IsDead())
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1);
                hasBeenSelectionned = true;
            }
            return true;
        }

        public string GetName()
        {
            return conversantName;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasBeenSelectionned && other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<Mover>().Cancel();
                other.GetComponent<PlayerConversant>().StartDialogue(this, dialogues);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Player walk away from NPC.
            if (hasBeenSelectionned && other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<PlayerConversant>().Quit();
                hasBeenSelectionned = false;
            }
        }
    }
}