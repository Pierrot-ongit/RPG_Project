using System;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "RPG/Abilities/Effects/SpawnProjectile", order = 0)]
    public class SpawnProjectileEffect : EffectStategy
    {
        [SerializeField][Min(0)] private float damage = 0f;
        [SerializeField] Projectile  projectileToSpawn;
        [SerializeField] bool isRightHand = true;
        [SerializeField] private bool useTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;

            if (useTargetPoint)
            {
                SpawProjectileForTargetPoint(data, spawnPosition);
            }
            else
            {
                SpawProjectilesForTargets(data, spawnPosition);
            }

            finished();
        }

        private void SpawProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectile = Instantiate(projectileToSpawn);
            projectile.transform.position = spawnPosition;
            projectile.SetTarget(data.GetTargetedPoint(), data.GetUser(), damage);
        }

        private void SpawProjectilesForTargets(AbilityData data, Vector3 spawnPosition)
        {
            foreach (GameObject target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();
                if (health)
                {
                    Projectile projectile = Instantiate(projectileToSpawn);
                    projectile.transform.position = spawnPosition;
                    projectile.SetTarget(target.GetComponent<Health>(), data.GetUser());
                }
            }
        }
    }
}