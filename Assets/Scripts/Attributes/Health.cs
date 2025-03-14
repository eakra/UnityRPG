using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes {

    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] UnityEvent<float> takeDamageEvent;
        
        LazyValue<float> currentHealth;
        LazyValue<float> maxHealth;

        bool isDead = false;
        
        BaseStats baseStats = null;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            currentHealth = new LazyValue<float>(GetInitialHealth);
            maxHealth = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            currentHealth.ForceInit();
            maxHealth.ForceInit();
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            if (baseStats != null)
            {
                baseStats.onLevelledUp += RecalculateHealthOnModifierChange;
            } 
        }

        private void OnDisable()
        {
            if (baseStats != null)
            {
                baseStats.onLevelledUp -= RecalculateHealthOnModifierChange;
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentHealth.value);
        }

        public bool IsDead() {
            return isDead;
        }

        public void RestoreFromJToken(JToken state)
        {
            currentHealth.value = state.ToObject<float>();
            if (currentHealth.value == 0) {
                Die();
            }
        }

        public void TakeDamage(GameObject instigator, float damage) {
            if (isDead) return;

            currentHealth.value = Mathf.Max(currentHealth.value - damage, 0);
            if (currentHealth.value == 0) {
                AwardExperience(instigator);
                Die();
            } else {
                takeDamageEvent.Invoke(damage);
            }
        }

        public float GetPercentage()
        {
            return currentHealth.value / maxHealth.value * 100;
        }

        public float GetCurrentHealth() 
        {
            return currentHealth.value;
        }

        public float GetMaxHealth()
        {
            return maxHealth.value;
        }

        public void RecalculateHealthOnModifierChange()
        {
            float currentMissingHealth = maxHealth.value - currentHealth.value;
            maxHealth.value = GetComponent<BaseStats>().GetStat(Stat.Health);
            currentHealth.value = maxHealth.value - currentMissingHealth;
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


