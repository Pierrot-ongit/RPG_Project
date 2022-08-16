using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using System;
using GameDevTV.Utils;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspiconTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrollingSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 3f; // Aggro other enemies.
        [SerializeField] private float enemeyHealthRegenOnReset = 20;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        int currentWaypointIndex = 1;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        //Vector3 guardPosition;
        LazyValue<Vector3> guardPosition;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetInitialPosition);
        }

        private void OnEnable()
        {
            guardPosition.ForceInit();
        }
        
        public void Reset()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(guardPosition.value);
            currentWaypointIndex = 1;
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            timeSinceArrivedAtWaypoint = Mathf.Infinity;
            if (!health.IsDead())
            {
                health.Heal(health.GetMaxHealthPoints() * enemeyHealthRegenOnReset / 100);
            }
        }
        
        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }
        
        public void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (player == null) return;
            if (health.IsDead()) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspiconTime)
            {
                // Suspicion state.
                SuspicionBehavior();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }



        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }



        private void AggrevateNearbyEnemies()
        {
           RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
           foreach (RaycastHit hit in hits)
           {
              AIController otherEnemy =  hit.transform.GetComponent<AIController>();
              if (otherEnemy != null)
              {
                  otherEnemy.Aggrevate();
              }
           }
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            fighter.Cancel();
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrollingSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerance;
        }

        private bool IsAggrevated()
        {
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance 
                   || timeSinceAggrevated < aggroCooldownTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, shoutDistance);
        }
    }
}

