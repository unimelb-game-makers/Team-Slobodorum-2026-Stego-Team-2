using UnityEngine;

namespace TeamSlobodorum.Entities.Spells
{
    [RequireComponent(typeof(ParticleSystem))]
    public class OneShotParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
        
        private void Update()
        {
            if (!_particleSystem.IsAlive(true)) 
            {
                Destroy(gameObject);
            }
        }
    }
}
