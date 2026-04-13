using TeamSlobodorum.Entities.Humanoid;
using TeamSlobodorum.Entities.Player;
using UnityEngine;

namespace TeamSlobodorum.Entities.HostileRobot
{
    public class HostileRobotEntity : LivingEntity
    {
        public float maxHitPoints = 100.0f;

        private HumanoidMovement _movement;
        private PlayerEntity _playerEntity;

        private const float FollowInterval = 1f;
        private float _followCounter;
        
        protected override void Awake()
        {
            base.Awake();
            
            _movement = GetComponent<HumanoidMovement>();
            
            MaxHitPoints = maxHitPoints;
            Died += OnDied;
        }

        protected override void Start()
        {
            base.Start();
            _playerEntity = GameObject.FindWithTag("Player").GetComponent<PlayerEntity>();
            _movement.IsSprinting = true;
        }

        private void Update()
        {
            if (_followCounter > 0)
            {
                _followCounter -= Time.deltaTime;
            }
            else
            {
                _movement.StartMoveTo(_playerEntity.transform.position);
                _followCounter = FollowInterval;
            }
        }

        private void OnDied()
        {
            Destroy(gameObject);
        }
    }
}