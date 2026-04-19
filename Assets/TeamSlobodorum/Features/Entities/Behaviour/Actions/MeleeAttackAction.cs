using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Melee Attack", story: "[Entity] melee attack", category: "Action/Entity",
        id: "4ac011ffb2e44071894f37691399f44a")]
    public partial class MeleeAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<Entity> entity;

        protected override Status OnStart()
        {
            if (entity.Value is IAttackable attackable)
            {
                attackable.Attack();
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