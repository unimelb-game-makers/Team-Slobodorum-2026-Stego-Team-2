using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start Move to Entity", story: "[Movement] starts move to [Entity]",
        category: "Action/Movement", id: "e823b83479d64dd78bdcb87d943c5a27")]
    public partial class StartMoveToEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<Entity> entity;

        protected override Status OnStart()
        {
            movement.Value.StartMoveTo(entity.Value.transform.position);
            return Status.Running;
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