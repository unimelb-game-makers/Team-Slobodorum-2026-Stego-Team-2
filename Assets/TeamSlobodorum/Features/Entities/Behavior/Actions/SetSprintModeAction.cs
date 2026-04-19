using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behavior.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Sprint Mode", story: "Set sprint mode in [Movement] to [SprintMode]",
        category: "Action/Movement", id: "c62796b0a525d0cc92e93042557e2eb4")]
    public partial class SetSprintModeAction : Action
    {
        [SerializeReference] public BlackboardVariable<Movement> movement;
        [SerializeReference] public BlackboardVariable<bool> sprintMode;

        protected override Status OnStart()
        {
            movement.Value.IsSprinting = sprintMode.Value;
            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}