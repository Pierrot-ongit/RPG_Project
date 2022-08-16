using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        private GameObject user;
        private Vector3 targetedPoint;
        private IEnumerable<GameObject> targets;
        private bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public GameObject GetUser()
        {
            return user;
        }

        public void SetUser(GameObject user)
        {
            this.user = user;
        }

        public void SetTargetedPoint(Vector3 targetedPoint)
        {
            this.targetedPoint = targetedPoint;
        }

        public Vector3 GetTargetedPoint()
        {
            return targetedPoint;
        }


        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }
    }
}