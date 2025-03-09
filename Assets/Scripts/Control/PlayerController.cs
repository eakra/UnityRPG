using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {   

        Mover mover;
        Fighter fighter;
        Health health;    

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();  
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }


        bool InteractWithMovement()
        {
            Ray ray = GetMouseRay();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        bool InteractWithCombat()
        {
            RaycastHit [] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (Input.GetMouseButton(0))
                {   
                    if (!fighter.CanAttack(target.gameObject)) continue;
                    fighter.Attack(target.gameObject);
                }
                return true; 
            }
            return false;
        }

        Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

    }
}
