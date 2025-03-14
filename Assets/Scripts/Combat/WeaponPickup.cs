using RPG.Control;
using UnityEngine;

namespace RPG.Combat {

    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<Fighter>());
            }
            return true;            
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
        
    }
}
