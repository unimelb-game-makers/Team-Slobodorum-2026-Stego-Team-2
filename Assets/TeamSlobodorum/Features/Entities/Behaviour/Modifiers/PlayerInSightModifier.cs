using System;
using TeamSlobodorum.AI;
using TeamSlobodorum.AI.Memory;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;

namespace TeamSlobodorum.Entities.Behaviour.Modifiers
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Player In Sight", story: "Set [PlayerEntity] to the player in sight",
        category: "Flow/Conditional", id: "10e9fcbb7a0c486494ec08569f25b124")]
    public partial class PlayerInSightModifier : Modifier
    {
        [SerializeReference, Tooltip("[Out Value] If a player entity is in sight, the field is assigned with it.")]
        public BlackboardVariable<Entity> playerEntity;

        private Brain _brain;

        protected override Status OnStart()
        {
            if (GameObject.TryGetComponent(out _brain))
            {
                playerEntity.Value = _brain.GetMemoryValueOrDefault(AllMemoryModuleTypes.PlayerInSight);
                if (playerEntity.Value)
                {
                    return StartNode(Child);
                }
            }

            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            return Child.CurrentStatus;
        }

        protected override void OnEnd()
        {
        }
    }
}