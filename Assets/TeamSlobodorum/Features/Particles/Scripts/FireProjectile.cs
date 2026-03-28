using TeamSlobodorum.Entities;
using TeamSlobodorum.Entities.Flammable;
using UnityEngine;

namespace TeamSlobodorum.Particles
{
    public class FireProjectile : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxTime = 1f;
        [SerializeField] private float disappearSpeed = 2f;
        [SerializeField] private float gravity = 3f;
        
        private float _currentTime;
        private float _currentScale = 1f;
        private bool _disappear;
        private bool _hasHit;
        
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            if (_disappear)
            {
                _currentScale = Mathf.Lerp(_currentScale, 0, Time.deltaTime * disappearSpeed);
                transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);

                if (_currentScale < 0.05f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                _currentTime += Time.deltaTime;

                if (_currentTime > maxTime)
                {
                    _disappear = true;
                }
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Flammable>(out var flammable))
            {
                foreach (var contact in collision.contacts)
                {
                    flammable.Ignite(contact.point);
                }
            }
        }
        
        private void FixedUpdate()
        {
            Rigidbody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
        }
    }
}
