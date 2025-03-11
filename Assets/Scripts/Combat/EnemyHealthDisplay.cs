using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;
using System;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Health target = fighter.GetTarget();
            Text textField = GetComponent<Text>();

            if (target == null) {
                textField.text = "N/A";
            } else {
                textField.text = String.Format("{0:0}/{1:0}", target.GetCurrentHealth(), target.GetMaxHealth());
            }
        }
    }
}
