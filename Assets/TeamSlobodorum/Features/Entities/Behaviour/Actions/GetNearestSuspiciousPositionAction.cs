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
    [NodeDescription(name: "Get Nearest Suspicious Position",
        story: "Get nearest suspicious position [Position] from [Brain]",
        category: "Action/Entity", id: "e29aae2696b44e0e8c08e777459ee7ff")]
    public partial class GetNearestSuspiciousPositionAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the nearest suspicious position.")]
        public BlackboardVariable<Vector3> position;

        [SerializeReference] public BlackboardVariable<Brain> brain;

        protected override Status OnStart()
        {
            if (brain.Value.TryGetMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition, out var pos))
            {
                position.Value = pos;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}