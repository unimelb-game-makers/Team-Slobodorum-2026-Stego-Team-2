using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;

namespace TeamSlobodorum.Entities.Behaviour.Modifiers
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "In Attack Range", story: "If [TargetEntity] is in the attack range of [Entity]",
        category: "Flow/Conditional", id: "2348c2e3b71169ea36a9c0156b109303")]
    public partial class InAttackRangeModifier : Modifier
    {
        [SerializeReference] public BlackboardVariable<Entity> targetEntity;
        [SerializeReference] public BlackboardVariable<Entity> entity;

        protected override Status OnStart()
        {
            if (entity.Value is IAttackable attackable)
            {
                if (Vector3.Distance(targetEntity.Value.transform.position, entity.Value.transform.position) <=
                    attackable.AttackRange)
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