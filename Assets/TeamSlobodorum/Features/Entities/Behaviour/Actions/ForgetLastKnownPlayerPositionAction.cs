using System;
using TeamSlobodorum.AI;
using TeamSlobodorum.AI.Memory;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Forget Last Known Player Position",
        story: "Forget the last known player position of [Brain]",
        category: "Action/Entity", id: "01f9a1c8f58b4dde8c56e41d38dd848f")]
    public partial class ForgetLastKnownPlayerPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Brain> brain;

        protected override Status OnStart()
        {
            brain.Value.ForgetMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition);
            return Status.Success;
        }
    }
}