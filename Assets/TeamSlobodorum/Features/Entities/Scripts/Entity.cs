using UnityEngine;

namespace TeamSlobodorum.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class Entity : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        
        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
