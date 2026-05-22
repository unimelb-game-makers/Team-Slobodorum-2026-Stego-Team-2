using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace TeamSlobodorum.Core.Behaviour.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Not Null",
        story: "[Value] is not null", category: "Conditions",
        id: "af1d460567fd48e685a02b4cc08d7cf6")]
    public partial class NotNullCondition : Condition
    {
        [SerializeReference] public BlackboardVariable value;

        public override bool IsTrue()
        {
            if (value.Type.IsValueType)
            {
                return true;
            }
            return value.ObjectValue is not null && !value.ObjectValue.Equals(null);
        }
    }
}