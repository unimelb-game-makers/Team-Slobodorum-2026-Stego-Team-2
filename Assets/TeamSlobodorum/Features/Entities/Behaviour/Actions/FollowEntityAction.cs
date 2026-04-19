using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Follow Entity", story: "[Movement] follows [Entity] at intervals of [Interval] seconds",
        category: "Action/Movement", id: "44bc1fce539f40f3a25fa4af42a25351")]
    public partial class FollowEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<Entity> entity;
        [SerializeReference] public BlackboardVariable<float> interval;

        private float _counter;

        protected override Status OnStart()
        {
            movement.Value.StartMoveTo(entity.Value.transform.position);
            _counter = interval.Value;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _counter -= Time.deltaTime;
            
            if (movement.Value.IsMoving)
            {
                if (_counter <= 0)
                {
                    movement.Value.StartMoveTo(entity.Value.transform.position);
                    _counter = interval.Value;
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