using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {

        [SerializeField] DamageText damageTextPrefab = null;
        
        public void Spawn(float damage)
        {
            Debug.Log("Damage from DamageSpawner.Spawn: " + damage);
            DamageText damageTextInstance = Instantiate<DamageText>(damageTextPrefab, transform);
            damageTextInstance.SetDamage(damage);
        }

    }
}
