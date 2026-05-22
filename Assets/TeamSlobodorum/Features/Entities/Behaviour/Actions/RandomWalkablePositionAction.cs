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
    [NodeDescription(name: "Random Walkable Position",
        story: "Get a random walkable position [Position] within a radius of [Radius] for [Movement]",
        category: "Action/Movement", id: "1e450a3916ac4d6c8c19812bf729dfa8")]
    public partial class RandomWalkablePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> position;
        [SerializeReference] public BlackboardVariable<float> radius;
        [SerializeReference] public BlackboardVariable<Movement> movement;

        protected override Status OnStart()
        {
            var randomDirection = Random.insideUnitSphere * radius.Value;
            var randomPosition = randomDirection + movement.Value.transform.position;

            if (NavMesh.SamplePosition(randomPosition, out var hit, 1f, NavMesh.AllAreas))
            {
                position.Value = hit.position;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}