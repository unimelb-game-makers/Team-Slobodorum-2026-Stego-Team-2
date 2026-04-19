using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start Move to Position", story: "[Movement] starts move to [Position]",
        category: "Action/Movement", id: "f92438c088984e4c28462d2d320baea1")]
    public partial class StartMoveToPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<Vector3> position;

        protected override Status OnStart()
        {
            movement.Value.StartMoveTo(position.Value);
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