using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] GameObject[] destroyedOnImpact = null;

        float damage = 0f;
        float lifeTime = 0f;
        Health target = null;

        GameObject instigator = null;


        private void Update()
        {
            if (target == null) return;
            if (isHoming && !target.IsDead()) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            lifeTime += Time.deltaTime;
            if (lifeTime > maxLifeTime) Destroy(gameObject);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            transform.LookAt(GetAimLocation());
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {   
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            
            target.TakeDamage(instigator, damage);
            speed = 0;
            
            if (hitEffect != null) Instantiate(hitEffect, GetAimLocation(), transform.rotation);


            foreach (GameObject toDestroy in destroyedOnImpact)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }

    }
}