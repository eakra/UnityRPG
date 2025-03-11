using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{

    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField] float timeBetweenAttacks = 1f;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        float attackSpeedFraction = 1f;

        Weapon currentWeapon = null;

        public void Start()
        {
            EquipWeapon(defaultWeapon);   
        }



        public void Update()
        {
            timeSinceLastAttack += Time.deltaTime; 

            if (target == null) return;
            if (target.IsDead()) return;

            if (!IsWithinRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, attackSpeedFraction);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }

        }



        public void SetAttackSpeedFraction(float fraction)
        {
            attackSpeedFraction = fraction;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            if (weapon == null) return;
            weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;

            float calculatedDamage = GetComponent<BaseStats>().GetStat(Stat.Health);

            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, calculatedDamage);
            } else {
                target.TakeDamage(gameObject, calculatedDamage);
            }
            
        }

        void Shoot()
        {
            Hit();
        }

        public bool IsWithinRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health health = GetComponent<Health>();
            return combatTarget != null && !health.IsDead();
        }

        public Health GetTarget()
        {
            return target;
        }

    }
}


