using System;
using System.Collections;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Modify Movement Speed Effect", menuName = "RPG/Abilities/Effects/ModifyMovementSpeedEffect", order = 0)]
    public class ModifyMovementSpeedEffect : EffectStategy
    {
        [SerializeField] private float activeTime = 2f;
        [Tooltip("The actual maxSpeed will be multiplied by this amount.")]
        [SerializeField] private float modifierSpeedCoefficient = 2;
        private bool isEffectActive;

        public override void StartEffect(AbilityData data, Action finished)
        {
            if (!isEffectActive)
            {
                isEffectActive = true;
                data.StartCoroutine(ModifySpeedEffect(data, finished));
            }
        }
        
        private IEnumerator ModifySpeedEffect(AbilityData data, Action finished)
        {
            
            // TODO Modify the deleguate the responsability to the mover.
            foreach (GameObject target in data.GetTargets())
            {
                Mover mover = target.GetComponent<Mover>();
                if (mover != null)
                {
                    float originalSpeed = mover.GetOriginalMaxSpeed();
                    mover.SetMaxSpeed(originalSpeed * modifierSpeedCoefficient);
                }
            }
            yield return new WaitForSeconds(activeTime);
            foreach (GameObject target in data.GetTargets())
            {
                Mover mover = target.GetComponent<Mover>();
                if (mover != null)
                {
                    float originalSpeed = mover.GetOriginalMaxSpeed();
                    mover.SetMaxSpeed(originalSpeed);
                }
            }
            isEffectActive = false;
            finished();
        }
    }
}