using System;
using TeamSlobodorum.AI;
using TeamSlobodorum.AI.Memory;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace TeamSlobodorum.Entities.Behaviour.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Suspicious Position Exists", story: "Suspicious position [Position] exists in [Brain]", category: "Conditions",
        id: "1249afbfb444424b8e144e7a972ed65a")]
    public partial class SuspiciousPositionExistsCondition : Condition
    {
        [SerializeReference, Tooltip("[Out Value] If a suspicious position exists, the field is assigned with it.")]
        public BlackboardVariable<Vector3> position;

        [SerializeReference] public BlackboardVariable<Brain> brain;

        public override bool IsTrue()
        {
            if (brain.Value.TryGetMemoryValue(AllMemoryModuleTypes.NearestSuspiciousPosition, out var value))
            {
                position.Value = value;
                return true;
            }

            return false;
        }
    }
}