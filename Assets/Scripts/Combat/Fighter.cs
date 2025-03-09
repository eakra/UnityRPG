using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{

    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        float attackSpeedFraction = 1f;

        public void SetAttackSpeedFraction(float fraction)
        {
            attackSpeedFraction = fraction;
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
            print("I'm gonna kick your ass!");
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

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        public bool IsWithinRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health health = GetComponent<Health>();
            return combatTarget != null && !health.IsDead();
        }

    }
}


