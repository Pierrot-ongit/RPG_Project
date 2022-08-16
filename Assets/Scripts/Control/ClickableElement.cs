using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{

    [RequireComponent(typeof(Collider))]
    public abstract  class ClickableElement : MonoBehaviour, IRaycastable
    {

        // Interaction properties.
        private bool hasBeenSelectionned = false;
        [SerializeField] float interactionRange = 3f;
        GameObject player;
        

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }


        public virtual CursorType GetCursorType()
        {
            return CursorType.None;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1);
                hasBeenSelectionned = true;
            }
            return true;
        }
        
        private void Update()
        {
            if (!hasBeenSelectionned) return;

            if (IsCloseEnoughToInteract())
            {
                player.GetComponent<Mover>().Cancel();
                hasBeenSelectionned = false;
                InteractionEffects();
            }
        }

        private bool IsCloseEnoughToInteract()
        {
            return Vector3.Distance(player.transform.position, transform.position) < interactionRange;
        }
        
        public virtual void InteractionEffects() {}
    }
}