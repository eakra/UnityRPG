using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text textField = null;

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void SetDamage(float damage)
        {   
            Debug.Log("Damage: " + damage);
            textField.text = String.Format("{0:0}", damage);
        }
    }
}
