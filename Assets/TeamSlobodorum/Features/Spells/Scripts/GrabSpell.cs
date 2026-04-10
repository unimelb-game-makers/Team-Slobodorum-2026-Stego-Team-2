using System;
using TeamSlobodorum.Entities;
using TeamSlobodorum.Entities.Humanoid;
using TeamSlobodorum.Particles;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public class GrabSpell : Spell
    {
        [Header("References")]
        [SerializeField] private LinkEffect linkEffectPrefab;
        [SerializeField] private GameObject projectilePrefab;
        
        [Header("Settings")]
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private LayerMask raycastLayers;

        private Camera _mainCamera;
        private Humanoid _humanoid;
        private LinkEffect _linkEffect;
        private Entity _grabbed;
        private float _grabDistance;
        private bool _useGravity;

        public override string SpellName => "Grab";

        public override bool Active => _grabbed != null;

        private void Awake()
        {
            _humanoid = GetComponent<Humanoid>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            
            _linkEffect = Instantiate(linkEffectPrefab, _humanoid.rightHand);
            _linkEffect.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (_grabbed)
            {
                var targetPos = _mainCamera.transform.position +
                                _mainCamera.transform.forward * _grabDistance;
                var direction = targetPos - _grabbed.Rigidbody.position;
                var speed = Math.Min(direction.magnitude * 10f, 30f);
                _grabbed.Rigidbody.linearVelocity = direction.normalized * speed;
            }
        }

        public void GrabEntity(Entity entity)
        {
            if (_grabbed == null)
            {
                _grabbed = entity;
                _grabDistance =
                    Vector3.Distance(_mainCamera.transform.position, entity.transform.position);
                _linkEffect.endPoint = entity.transform;
                _linkEffect.gameObject.SetActive(true);

                _useGravity = _grabbed.Rigidbody.useGravity;
                _grabbed.Rigidbody.useGravity = false;
            }
        }

        public override void Use()
        {
            if (_grabbed)
            {
                return;
            }

            var projectileObject = Instantiate(projectilePrefab, _humanoid.rightHand.position,
                Quaternion.LookRotation(_mainCamera.transform.forward));
            var projectile = projectileObject.GetComponent<GrabProjectile>();
            projectile.Spell = this;

            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out var hit,
                    maxDistance, raycastLayers))
            {
                var direction = (hit.point - _humanoid.rightHand.position).normalized;
                projectile.transform.rotation = Quaternion.LookRotation(direction);
                projectile.Rigidbody.AddForce(direction * projectileSpeed, ForceMode.Impulse);
            }
            else
            {
                projectile.Rigidbody.AddForce(_mainCamera.transform.forward * projectileSpeed, ForceMode.Impulse);
            }
        }

        public override void CancelEffect()
        {
            if (_grabbed != null)
            {
                if (_grabbed.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = _useGravity;
                }

                _grabbed = null;
                _linkEffect.gameObject.SetActive(false);
            }
        }
    }
}