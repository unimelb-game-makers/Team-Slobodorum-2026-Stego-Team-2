using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack", story: "[Entity] attacks [TargetEntity]", category: "Action/Entity",
        id: "4ac011ffb2e44071894f37691399f44a")]
    public partial class AttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<Entity> entity;
        [SerializeReference] public BlackboardVariable<Entity> targetEntity;

        protected override Status OnStart()
        {
            if (entity.Value is IAttackable attackable)
            {
                attackable.Attack(targetEntity.Value);
                return Status.Success;
            }

            return Status.Failure;
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