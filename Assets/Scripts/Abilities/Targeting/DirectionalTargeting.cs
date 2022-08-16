using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "RPG/Abilities/Targeting/Directional", order = 0)]
    public class DirectionalTargeting : TargetingStategy
    {

        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float groundOffset;
        [SerializeField] private bool isDelayed = false;
 
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController  playerController = data.GetUser().GetComponent<PlayerController>();
           playerController.StartCoroutine(Targeting(data, playerController, finished));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            if (isDelayed)
            {
                playerController.enabled = false;
                while (!data.IsCancelled())
                {
                    Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                    RaycastHit raycastHit;
                    Ray ray = PlayerController.GetMouseRay();
                    if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Absord the whole mouse click.
                            yield return new WaitWhile(() => Input.GetMouseButton(0));
                            data.SetTargetedPoint(raycastHit.point +  ray.direction * groundOffset / ray.direction.y);
                            break; //Normal exit of the loop without exiting the function.
                        }
                    }
                    yield return null; // Wait until next frame.
                }

                playerController.enabled = true;
            }
            else
            {
                RaycastHit raycastHit;
                Ray ray = PlayerController.GetMouseRay();
                if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
                {
                    data.SetTargetedPoint(raycastHit.point +  ray.direction * groundOffset / ray.direction.y);
                }
            }
            
            
            finished();
        }
    }
}