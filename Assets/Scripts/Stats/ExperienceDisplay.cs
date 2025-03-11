using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;

        private void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();   
        }

        private void Update()
        {
            GetComponent<Text>().text = Mathf.FloorToInt(experience.GetExperience()) + "";
        }
    }
}
