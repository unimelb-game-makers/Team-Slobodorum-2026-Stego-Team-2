using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start Moving to Random Position", story: "[Movement] starts moving to a random position within a radius of [Radius]",
        category: "Action/Movement", id: "ebf16a72fbe8406ca3f282b4fd03022d")]
    public partial class StartMovingToRandomPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<float> radius;

        protected override Status OnStart()
        {
            var randomDirection = Random.insideUnitSphere * radius.Value;
            var randomPosition = randomDirection + movement.Value.transform.position;
            
            if (NavMesh.SamplePosition(randomPosition, out var hit, 1f, NavMesh.AllAreas))
            {
                movement.Value.StartMovingTo(hit.position);
                return Status.Running;
            }
            
            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            return movement.Value.IsMoving ? Status.Running :
                movement.Value.LastMoveSucceeded ? Status.Success : Status.Failure;
        }

        protected override void OnEnd()
        {
        }
    }
}