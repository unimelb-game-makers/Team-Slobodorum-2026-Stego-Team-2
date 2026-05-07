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
    [Condition(name: "Player In Sight", story: "[PlayerEntity] is in sight of [Entity]", category: "Conditions",
        id: "45a5e0015bb0fc1b9007d7446fb20015")]
    public partial class PlayerInSightCondition : Condition
    {
        [SerializeReference, Tooltip("[Out Value] If a player entity is in sight, the field is assigned with it.")]
        public BlackboardVariable<PlayerEntity> playerEntity;

        [SerializeReference] public BlackboardVariable<Entity> entity;
        
        private Brain _brain;

        public override void OnStart()
        {
            entity.Value.TryGetComponent(out _brain);
        }

        public override bool IsTrue()
        {
            if (_brain)
            {
                playerEntity.Value = _brain.GetMemoryValueOrDefault(AllMemoryModuleTypes.PlayerInSight);
                return playerEntity.Value;
            }

            return false;
        }
    }
}