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
    [NodeDescription(name: "Get Time Last Saw Player",
        story: "Get the time last saw player [Time] from [Brain]",
        category: "Action/Entity", id: "29f058a6bdc04199b3b61fa3aae3d9ff")]
    public partial class GetLastTimeSawPlayerPositionAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the time last saw the player.")]
        public BlackboardVariable<float> time;

        [SerializeReference] public BlackboardVariable<Brain> brain;

        protected override Status OnStart()
        {
            if (brain.Value.TryGetMemoryValue(AllMemoryModuleTypes.TimeLastSawPlayer, out var t))
            {
                time.Value = t;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}