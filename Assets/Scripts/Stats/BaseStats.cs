using System;
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

        int currentLevel = 1;

        public event Action onLevelledUp;

        private void Start()
        {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null) 
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel) {
                currentLevel = newLevel;
                onLevelledUp();
            }

        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }        

        public int GetLevel()
        {
            if (currentLevel < 1) { CalculateLevel(); }
            return currentLevel;
        }
        

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float[] levelUpArray = progression.GetLevels(Stat.ExperienceToLevelUp,characterClass);
            float experiencePoints = GetComponent<Experience>().GetExperience();

            int level = 1;

            while (level <= levelUpArray.Length && experiencePoints >= levelUpArray[level - 1]) {level += 1;}

            Debug.Log("Current level is " + level);

            return level;
        } 


    }
}
