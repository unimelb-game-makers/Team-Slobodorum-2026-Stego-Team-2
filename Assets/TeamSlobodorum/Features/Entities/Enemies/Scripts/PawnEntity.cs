using System;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Health;
using UnityEngine;

namespace TeamSlobodorum.Entities.Enemies
{
    public class PawnEntity : EnemyEntity, IAttackable
    {
        [SerializeField] private float meleeAttackRange = 1f;

        private ChessPieceMovement _movement;
        private readonly Collider[] _hitColliders = new Collider[5];
        
        public float AttackRange => meleeAttackRange;

        protected override void Awake()
        {
            base.Awake();
            _movement = GetComponent<ChessPieceMovement>();
        }

        public void Attack(Entity target)
        {
            _movement.StartMeleeAttack();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var playerEntity = collision.gameObject.GetComponent<PlayerEntity>();
                playerEntity.HealthManager.TakeDamage(10f, DamageType.Physical);
            }
        }
        
        private void CauseDamage()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, meleeAttackRange, _hitColliders,
                LayerMask.GetMask("Player"));

            for (var i = 0; i < size; i++)
            {
                var hitCollider = _hitColliders[i];
                var directionToObject = (hitCollider.transform.position - transform.position).normalized;

                // Dot product check: Is it in front (forward) of this object?
                if (Vector3.Dot(transform.forward, directionToObject) > 0)
                {
                    var playerEntity = hitCollider.GetComponent<PlayerEntity>();
                    playerEntity.HealthManager.TakeDamage(10, DamageType.Physical);
                }
            }
        }
    }
}