using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Humanoid.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start Move to Position", story: "[humanoidMovement] starts move to [position]",
        category: "Action/Movement", id: "f92438c088984e4c28462d2d320baea1")]
    public partial class StartMoveToPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<HumanoidMovement> humanoidMovement;
        [SerializeReference] public BlackboardVariable<Vector3> position;

        protected override Status OnStart()
        {
            humanoidMovement.Value.StartMoveTo(position.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return humanoidMovement.Value.IsMoving ? Status.Running :
                humanoidMovement.Value.LastMoveSucceeded ? Status.Success : Status.Failure;
        }

        protected override void OnEnd()
        {
        }
    }
}