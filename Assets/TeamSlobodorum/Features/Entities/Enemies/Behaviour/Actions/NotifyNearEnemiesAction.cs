using System;
using TeamSlobodorum.AI.Memory;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Enemies.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Notify Near Enemies", story: "[EnemyEntity] notifies near enemies within a radius of [Radius]",
        category: "Action/Entity", id: "3218ae282b994de49602223b2f0e189a")]
    public class NotifyNearEnemiesAction : Action
    {
        [SerializeReference] public BlackboardVariable<EnemyEntity> enemyEntity;
        [SerializeReference] public BlackboardVariable<float> radius;

        private readonly Collider[] _hitResults = new Collider[10];
        private float _timeLastNotify;
        private const float NotifyInterval = 1f;
        
        protected override Status OnStart()
        {
            if (_timeLastNotify == 0)
            {
                _timeLastNotify = Time.time;
            }

            var now = Time.time;
            if (now - _timeLastNotify < NotifyInterval)
            {
                return Status.Success;
            }
            _timeLastNotify = now;
            
            var size = Physics.OverlapSphereNonAlloc(enemyEntity.Value.transform.position, radius, _hitResults,
                LayerMask.GetMask("Living Entity"));
            var brain = enemyEntity.Value.Brain;

            if (!brain.TryGetMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition, out var playerPosition))
            {
                return Status.Success;
            }
            var t0 = brain.GetMemoryValue(AllMemoryModuleTypes.TimeLastSawPlayer);
            
            for (var i = 0; i < size; i++)
            {
                if (_hitResults[i].TryGetComponent<EnemyEntity>(out var other))
                {
                    var otherBrain = other.Brain;
                    
                    if (otherBrain.TryGetMemoryValue(AllMemoryModuleTypes.TimeLastSawPlayer, out var t1))
                    {
                        if (t1 < t0)
                        {
                            otherBrain.RememberMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition, playerPosition);
                            otherBrain.RememberMemoryValue(AllMemoryModuleTypes.TimeLastSawPlayer, t0);
                        }
                    }
                    else
                    {
                        otherBrain.RememberMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition, playerPosition);
                        otherBrain.RememberMemoryValue(AllMemoryModuleTypes.TimeLastSawPlayer, t0);
                    }

                    if (other.EnemyState != EnemyState.Attack)
                    {
                        other.EnemyState = EnemyState.Attack;
                    }
                }
            }
            
            return Status.Success;
        }
    }
}