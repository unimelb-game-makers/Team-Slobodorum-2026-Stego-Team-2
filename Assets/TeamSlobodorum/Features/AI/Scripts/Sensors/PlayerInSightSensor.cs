using TeamSlobodorum.AI.Memory;
using TeamSlobodorum.Entities;
using TeamSlobodorum.Entities.Player;
using UnityEngine;

namespace TeamSlobodorum.AI.Sensors
{
    [CreateAssetMenu(menuName = "Sensors/Player In Sight Sensor")]
    public class PlayerInSightSensor : Sensor
    {
        public float viewRadius = 20f;
        public float viewAngle = 120f;
        public LayerMask layersToHit;

        private LivingEntity _livingEntity;
        private PlayerEntity _playerEntity;

        public override void Initialize(Brain brain)
        {
            _livingEntity = brain.GetComponent<LivingEntity>();
            var player = GameObject.FindWithTag("Player");
            if (player)
            {
                _playerEntity = player.GetComponent<PlayerEntity>();
            }
        }

        public override void Tick(Brain brain)
        {
            if (CanSeePlayer(brain))
            {
                brain.RememberMemoryValue(AllMemoryModuleTypes.PlayerInSight, _playerEntity);
                brain.RememberMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition,
                    _playerEntity.transform.position);
            }
            else
            {
                brain.ForgetMemoryValue(AllMemoryModuleTypes.PlayerInSight);
            }
        }

        private bool CanSeePlayer(Brain brain)
        {
            // Distance Check
            var distanceToPlayer = Vector3.Distance(brain.transform.position, _playerEntity.transform.position);
            if (distanceToPlayer > viewRadius) return false;

            // Field of View (Angle) Check
            var directionToPlayer = (_playerEntity.eyes.position - _livingEntity.eyes.position).normalized;
            if (Vector3.Angle(_livingEntity.eyes.forward, directionToPlayer) > viewAngle / 2) return false;

            // Line of Sight (Raycast) Check
            if (Physics.Raycast(_livingEntity.eyes.position, directionToPlayer, distanceToPlayer, layersToHit))
            {
                return false; // Something is in the way
            }

            return true;
        }
    }
}