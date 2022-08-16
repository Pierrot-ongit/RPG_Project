using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] bool isHoming = false;
        [SerializeField] float speed = 5f;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;
        [SerializeField] UnityEvent ontHit;

        Health target = null;
        float damage = 0;
        GameObject instigator = null;
        Vector3 targetPoint;


        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target !=null && isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }
        
        public void SetTarget(GameObject instigator, float damage, Health target = null, Vector3 targetPoint = default)
        {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
            this.targetPoint = targetPoint;

            Destroy(gameObject, maxLifeTime);
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            if (target == null)
            {
                return targetPoint;
            }
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            // We collided with ourselves (player).
            if (other.gameObject == instigator) return;
            
            Health health = other.GetComponent<Health>();
            // If we have a "living" target, we ignore this collision.
            if (target != null && health != target) return;

            // We collided with something that can't be damaged (not living).
            if (health == null || health.IsDead()) return;
            
            speed = 0;
            health.TakeDamage(instigator, this.damage);
            ontHit.Invoke();

            if (hitEffect != null)
            {
                GameObject hitEffectInstance = Instantiate(hitEffect, GetAimLocation(), Quaternion.identity);
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}

