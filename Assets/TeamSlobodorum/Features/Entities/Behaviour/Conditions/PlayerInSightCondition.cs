using System;
using TeamSlobodorum.AI;
using TeamSlobodorum.AI.Memory;
using TeamSlobodorum.Entities.Player;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace TeamSlobodorum.Entities.Behaviour.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Player In Sight", story: "[PlayerEntity] is in the sight of [Brain]", category: "Conditions",
        id: "45a5e0015bb0fc1b9007d7446fb20015")]
    public partial class PlayerInSightCondition : Condition
    {
        [SerializeReference, Tooltip("[Out Value] If a player entity is in sight, the field is assigned with it.")]
        public BlackboardVariable<PlayerEntity> playerEntity;

        [SerializeReference] public BlackboardVariable<Brain> brain;

        public override bool IsTrue()
        {
            if (brain.Value.TryGetMemoryValue(AllMemoryModuleTypes.PlayerInSight, out var value))
            {
                playerEntity.Value = value;
                return true;
            }

            return false;
        }
    }
}