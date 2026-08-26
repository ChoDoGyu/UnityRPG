using UnityEngine;

namespace UnityRPG.Core
{
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class GameplayRootLifetime : MonoBehaviour
    {
        private static GameplayRootLifetime instance;

        public static bool HasInstance => instance != null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        public static void DestroyCurrent()
        {
            if (instance != null)
                Destroy(instance.gameObject);
        }
    }
}