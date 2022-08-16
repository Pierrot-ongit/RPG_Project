using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float maxNavPathLength = 40f;
        private float originalMaxSpeed;
        
        NavMeshAgent navMeshAgent;
        Animator animator;
        Health health;
        Ray lastRay;

        // Start is called before the first frame update
        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
            originalMaxSpeed = maxSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public float GetOriginalMaxSpeed()
        {
            return originalMaxSpeed;
        }
        
        public float GetMaxSpeed()
        {
            return maxSpeed;
        }
        
        public void SetMaxSpeed(float speed)
        {
            maxSpeed = speed;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            // Do we have a complete AND short path available ?
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;
            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.SetDestination(destination);
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }
        
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            /* Same fonctionnality as struct serializable but with dictionnary.
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            */
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            /*
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            navMeshAgent.Warp(((SerializableVector3)data["position"]).ToVector());
            Vector3 ea = ((SerializableVector3)data["rotation"]).ToVector();
            transform.eulerAngles = ea;
            */
            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.Warp(data.position.ToVector());
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}

