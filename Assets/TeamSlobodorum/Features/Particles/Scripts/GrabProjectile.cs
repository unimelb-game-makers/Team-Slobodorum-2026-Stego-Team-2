using TeamSlobodorum.Entities;
using TeamSlobodorum.Spells;
using UnityEngine;

namespace TeamSlobodorum.Particles
{
    public class GrabProjectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject hitPrefab;

        [Header("Settings")]
        [SerializeField] private float maxTime = 5f;
        [SerializeField] private float disappearSpeed = 5f;
        [SerializeField] private float gravity = 3f;

        private float _currentTime;
        private float _currentScale = 1f;
        private bool _disappear;
        private bool _hasHit;

        public GrabSpell Spell { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Rigidbody.linearVelocity != Vector3.zero)
            {
                transform.forward = Rigidbody.linearVelocity.normalized;
            }
            
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

        private void FixedUpdate()
        {
            Rigidbody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_hasHit && other.gameObject.TryGetComponent<Entity>(out var entity))
            {
                Spell.GrabEntity(entity);
                _hasHit = true;
            }
            Instantiate(hitPrefab, transform.position, transform.rotation);
            _disappear = true;
        }
    }
}