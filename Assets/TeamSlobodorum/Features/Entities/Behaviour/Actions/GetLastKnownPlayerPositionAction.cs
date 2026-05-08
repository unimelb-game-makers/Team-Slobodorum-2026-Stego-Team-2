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
    [NodeDescription(name: "Get Last Known Player Position",
        story: "Get the last known player position [Position] of [Brain]",
        category: "Action/Entity", id: "087dfcddf00b4781a3c4c4ac8f74bef6")]
    public partial class GetLastKnownPlayerPositionAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the last known position of the player.")]
        public BlackboardVariable<Vector3> position;

        [SerializeReference] public BlackboardVariable<Brain> brain;

        protected override Status OnStart()
        {
            if (brain.Value.TryGetMemoryValue(AllMemoryModuleTypes.LastKnownPlayerPosition, out var pos))
            {
                position.Value = pos;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}