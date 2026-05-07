using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace TeamSlobodorum.Entities.Behaviour.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "In Attack Range", story: "[TargetEntity] is in the attack range of [Entity]",
        category: "Conditions", id: "889b9ad1a3364460bd8fdba34450bbff")]
    public partial class InAttackRangeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<Entity> targetEntity;
        [SerializeReference] public BlackboardVariable<Entity> entity;

        public override bool IsTrue()
        {
            if (entity.Value is IAttackable attackable)
            {
                if (Vector3.Distance(targetEntity.Value.transform.position, entity.Value.transform.position) <=
                    attackable.AttackRange)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}