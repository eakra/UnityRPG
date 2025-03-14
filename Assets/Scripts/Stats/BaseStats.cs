using System;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats 
{
    public class BaseStats : MonoBehaviour 
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass; 
        [SerializeField] Progression progression = null;

        LazyValue<int> currentLevel;

        public event Action onLevelledUp;
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }
        

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
                onLevelledUp();
            }

        }

        public float GetStat(Stat stat)
        {
            return (progression.GetStat(stat, characterClass, GetLevel()) + GetAdditiveModifier(stat)) * GetPercentageModifier(stat);
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) 
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 100;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total / 100;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }
        

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float[] levelUpArray = progression.GetLevels(Stat.ExperienceToLevelUp,characterClass);
            float experiencePoints = GetComponent<Experience>().GetExperience();

            int level = 1;

            while (level <= levelUpArray.Length && experiencePoints >= levelUpArray[level - 1]) {level += 1;}

            return level;
        } 


    }
}
