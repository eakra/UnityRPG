using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using UnityEngine.AI;
using Newtonsoft.Json.Linq;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        NavMeshAgent navMeshAgent; 
        Health health;
        float maxSpeed = 6.0f;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        private void Start(){
        }

        private void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void Cancel(){
            navMeshAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction = 1f)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }
        

        public void MoveTo(Vector3 destination, float speedFraction = 1f)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float forwardSpeed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", forwardSpeed);
        }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = false;
            transform.position = state.ToVector3();
            navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}


