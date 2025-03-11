using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats 
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression")]
    public class Progression : ScriptableObject
    {

        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;

        public void Awake()
        {
            BuildLookup();   
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            Debug.Log("Passed in the following parameters:" + stat + " " + characterClass + " " + level);
            BuildLookup();
            //ProgressionCharacterClass classResult = Array.Find(characterClasses, x => x.characterClass == characterClass);
            //ProgressionStat statResult = Array.Find(classResult.stats, x => x.stat == stat);
            return lookUpTable[characterClass][stat][level - 1];
        }

        public float[] GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            return lookUpTable[characterClass][stat];
        }

        private void BuildLookup()
        {
            if (lookUpTable != null) return;

            lookUpTable = new Dictionary <CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass charClass in characterClasses) 
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progStat in charClass.stats)
                {
                    statLookUpTable[progStat.stat] = progStat.levels;
                }
                lookUpTable[charClass.characterClass] = statLookUpTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;

        }
    }

}