using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{

    public class Fighter : MonoBehaviour, IAction, IModifierProvider
    {

        [SerializeField] float timeBetweenAttacks = 1f;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        float attackSpeedFraction = 1f;

        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        public void Start()
        {
            currentWeapon.ForceInit();  
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
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;

            float calculatedDamage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, calculatedDamage);
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
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.value.GetRange();
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
             
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentage();
            }
        }
    }
}


