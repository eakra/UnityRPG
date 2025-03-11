using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Attributes {

    public class Health : MonoBehaviour, IJsonSaveable
    {
        float currentHealth = -1;
        float maxHealth = -1;

        bool isDead = false;
        
        BaseStats baseStats = null;

        private void Start()
        {
            baseStats = GetComponent<BaseStats>();

            if (currentHealth < 0) { currentHealth = baseStats.GetStat(Stat.Health); }
            maxHealth = baseStats.GetStat(Stat.Health);
            baseStats.onLevelledUp += RecalculateHealthOnModifierChange;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentHealth);
        }

        public bool IsDead() {
            return isDead;
        }

        public void RestoreFromJToken(JToken state)
        {
            currentHealth = state.ToObject<float>();
            if (currentHealth == 0) {
                Die();
            }
        }

        public void TakeDamage(GameObject instigator, float damage) {
            if (isDead) return;

            print(gameObject.name + " took " + damage + " damage.");

            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth == 0) {
                AwardExperience(instigator);
                Die();
            }
        }

        public float GetPercentage()
        {
            return currentHealth / maxHealth * 100;
        }

        public float GetCurrentHealth() 
        {
            return currentHealth;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public void RecalculateHealthOnModifierChange()
        {
            float currentMissingHealth = maxHealth - currentHealth;
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            currentHealth = maxHealth - currentMissingHealth;
        }

        private void Die() {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            float xpReward = GetComponent<BaseStats>().GetStat(Stat.ExperienceReward);
            experience.GainExperience(xpReward);
        }
    }

}


