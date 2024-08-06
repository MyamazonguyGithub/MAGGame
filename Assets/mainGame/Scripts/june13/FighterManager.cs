
using UnityEngine;
namespace Photon.Pun.Demo.Asteroids
{
    public class FighterManager : MonoBehaviour
    {
        public static FighterManager Instance;
        public Fighter fighter;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetFighter(Fighter fighterInstance)
        {
            fighter = fighterInstance;
        }

        public void SetFighterName(string fighterName)
        {
            if (fighter != null)
            {
                fighter.fighterName = fighterName;
            }
            else
            {
                Debug.LogWarning("Fighter instance not set in FighterManager.");
            }
            Debug.Log("fighterName: " + fighterName);
        }

        public void SetFighterNameFromFirebase()
        {
            if (FirebaseManager.Instance != null && FirebaseManager.Instance.User != null)
            {
                string displayName = FirebaseManager.Instance.User.DisplayName;
                SetFighterName(displayName);
            }
            else
            {
                Debug.LogWarning("FirebaseManager instance or User is null.");
            }
        }
    }
}