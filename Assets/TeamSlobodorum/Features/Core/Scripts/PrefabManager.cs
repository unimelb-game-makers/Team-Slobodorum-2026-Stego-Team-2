using TeamSlobodorum.Particles;
using UnityEngine;

namespace TeamSlobodorum.Core
{
    public class PrefabManager : MonoBehaviour
    {
        public Fire firePrefab;
        public Material burnMarkMaterial;
        
        public static PrefabManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
