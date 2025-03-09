using System;
using UnityEngine;

namespace RPG.Core{

    public class Health : MonoBehaviour
    {
        [SerializeField] float health = 100f;

        bool isDead = false;

        public bool IsDead() {
            return isDead;
        }

        public void TakeDamage(float damage) {
            if (isDead) return;

            health = Math.Max(health-damage, 0);
            if (health == 0) {
                Die();
            }
        }

        private void Die() {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }

}


