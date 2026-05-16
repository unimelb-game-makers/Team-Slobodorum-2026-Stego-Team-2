using TeamSlobodorum.AI.Memory;
using TeamSlobodorum.Entities;
using UnityEngine;

namespace TeamSlobodorum.AI.Sensors
{
    [CreateAssetMenu(menuName = "Sensors/Suspicious Behaviour Sensor")]
    public class SuspiciousBehaviourSensor : Sensor
    {
        public float viewRadius = 20f;
        public float viewAngle = 120f;
        public LayerMask layersToHit;

        private readonly Collider[] _hitResults = new Collider[10];
        private LivingEntity _livingEntity;

        public override void OnStart(Brain brain)
        {
            _livingEntity = brain.GetComponent<LivingEntity>();
        }

        public override void OnUpdate(Brain brain)
        {
            var size = Physics.OverlapSphereNonAlloc(brain.transform.position, viewRadius, _hitResults,
                LayerMask.GetMask("Player", "Player Projectiles"));
            var validSize = 0;
            for (var i = 0; i < size; i++)
            {
                var position = _hitResults[i].transform.position;
                if (!CanSeePosition(brain, position)) continue;
                validSize++;

                if (brain.TryGetMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition, out var prevPosition))
                {
                    var distance = Vector3.Distance(_livingEntity.eyes.position, position);
                    var prevDistance = Vector3.Distance(_livingEntity.eyes.position, prevPosition);
                    if (distance > prevDistance)
                    {
                        brain.RememberMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition, position);
                    }
                }
                else
                {
                    brain.RememberMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition, position);
                }
            }

            if (validSize == 0)
            {
                brain.ForgetMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition);
            }
        }

        private bool CanSeePosition(Brain brain, Vector3 position)
        {
            // Distance Check
            var distanceToPlayer = Vector3.Distance(brain.transform.position, position);
            if (distanceToPlayer > viewRadius) return false;

            // Field of View (Angle) Check
            var directionToPlayer = (position - _livingEntity.eyes.position).normalized;
            if (Vector3.Angle(_livingEntity.eyes.forward, directionToPlayer) > viewAngle / 2) return false;

            // Line of Sight (Raycast) Check
            if (Physics.Raycast(_livingEntity.eyes.position, directionToPlayer, distanceToPlayer, layersToHit,
                    QueryTriggerInteraction.Ignore))
            {
                return false; // Something is in the way
            }

            return true;
        }
    }
}