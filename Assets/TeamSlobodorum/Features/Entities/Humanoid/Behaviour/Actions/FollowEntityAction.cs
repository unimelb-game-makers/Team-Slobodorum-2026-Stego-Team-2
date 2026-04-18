using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Humanoid.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Follow Entity", story: "[humanoidMovement] follows [entity] at intervals of [interval] seconds",
        category: "Action/Movement", id: "44bc1fce539f40f3a25fa4af42a25351")]
    public partial class FollowEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<HumanoidMovement> humanoidMovement;
        [SerializeReference] public BlackboardVariable<Entity> entity;
        [SerializeReference] public BlackboardVariable<float> interval;

        private float _counter;

        protected override Status OnStart()
        {
            humanoidMovement.Value.StartMoveTo(entity.Value.transform.position);
            _counter = interval.Value;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _counter -= Time.deltaTime;
            
            if (humanoidMovement.Value.IsMoving)
            {
                if (_counter <= 0)
                {
                    humanoidMovement.Value.StartMoveTo(entity.Value.transform.position);
                    _counter = interval.Value;
                }
                return Status.Running;
            }
            
            return humanoidMovement.Value.LastMoveSucceeded ? Status.Success : Status.Failure;
        }

        protected override void OnEnd()
        {
        }
    }
}