using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Particles;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public class FireSpell : Spell
    {
        [Header("References")]
        [SerializeField] private GameObject projectilePrefab;

        [Header("Settings")]
        [SerializeField] private float maxDistance = 20f;

        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private LayerMask raycastLayers;

        private Camera _mainCamera;
        private Spellcaster _spellcaster;

        public override string SpellName => "Fire";
        public override bool Active => false;

        private void Awake()
        {
            _spellcaster = GetComponent<Spellcaster>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public override void Use()
        {
            var projectileObject = Instantiate(projectilePrefab, _spellcaster.hand.transform.position,
                Quaternion.LookRotation(_mainCamera.transform.forward));
            var projectile = projectileObject.GetComponent<FireProjectile>();

            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out var hit,
                    maxDistance, raycastLayers))
            {
                var direction = (hit.point - _spellcaster.hand.transform.position).normalized;
                projectile.transform.rotation = Quaternion.LookRotation(direction);
                projectile.Rigidbody.AddForce(direction * projectileSpeed, ForceMode.Impulse);
            }
            else
            {
                projectile.Rigidbody.AddForce(_mainCamera.transform.forward * projectileSpeed, ForceMode.Impulse);
            }
        }
    }
}