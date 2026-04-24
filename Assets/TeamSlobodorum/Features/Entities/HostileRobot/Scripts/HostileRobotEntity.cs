using TeamSlobodorum.Entities.Humanoid;
using TeamSlobodorum.Entities.Player;
using UnityEngine;

namespace TeamSlobodorum.Entities.HostileRobot
{
    public class HostileRobotEntity : LivingEntity, IAttackable
    {
        [SerializeField] private float meleeAttackRange = 1f;

        private HumanoidMovement _movement;
        private readonly Collider[] _hitColliders = new Collider[5];

        protected override void Awake()
        {
            base.Awake();

            _movement = GetComponent<HumanoidMovement>();
            Died += OnDied;
        }

        private void OnDied()
        {
            Destroy(gameObject);
        }

        public float AttackRange => meleeAttackRange;

        public void Attack(Entity target)
        {
            _movement.StartMeleeAttack();
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
                    playerEntity.TakeDamage(10);
                }
            }
        }
    }
}