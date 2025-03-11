using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {

    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))            
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            Fader fader = FindObjectsByType<Fader>(FindObjectsSortMode.None)[0];
            DontDestroyOnLoad(gameObject);
            yield return fader.FadeOut(2f);

            SavingWrapper wrapper = FindObjectsByType<SavingWrapper>(FindObjectsSortMode.None)[0];
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            wrapper.Load();
             
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();
            yield return fader.FadeIn(1.5f);

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsByType<Portal>(FindObjectsSortMode.None);
            foreach (Portal portal in portals)
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
                
            }
            return null;
        }
    }
}
