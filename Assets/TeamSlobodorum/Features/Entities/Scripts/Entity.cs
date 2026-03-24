using UnityEngine;

namespace TeamSlobodorum.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class Entity : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
