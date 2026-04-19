using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Follow Entity",
        story:
        "[Movement] follows [TargetEntity] with a stopping distance [StoppingDistance]",
        category: "Action/Movement", id: "44bc1fce539f40f3a25fa4af42a25351")]
    public partial class FollowEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<Entity> targetEntity;
        [SerializeReference] public BlackboardVariable<float> stoppingDistance;
        
        private const float UpdateInterval = 0.1f;

        private float _counter;

        protected override Status OnStart()
        {
            movement.Value.StartMovingTo(targetEntity.Value.transform.position, stoppingDistance.Value);
            _counter = UpdateInterval;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _counter -= Time.deltaTime;

            if (movement.Value.IsMoving)
            {
                if (_counter <= 0)
                {
                    movement.Value.StartMovingTo(targetEntity.Value.transform.position, stoppingDistance.Value);
                    _counter = UpdateInterval;
                }

                return Status.Running;
            }

            return movement.Value.LastMoveSucceeded ? Status.Success : Status.Failure;
        }

        protected override void OnEnd()
        {
        }
    }
}