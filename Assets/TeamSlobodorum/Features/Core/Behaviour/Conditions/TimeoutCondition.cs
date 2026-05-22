using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace TeamSlobodorum.Core.Behaviour.Conditions
{
    [BlackboardEnum]
    public enum TimeoutOperator
    {
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
    }

    [Serializable, GeneratePropertyBag]
    [Condition(name: "Timeout",
        story: "The time elapsed since [LastTime] is [TimeoutOperator] [Timeout] seconds", category: "Conditions",
        id: "c25db7b868544109b6409dff8598a840")]
    public partial class TimeoutCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<float> lastTime;
        [SerializeReference] public BlackboardVariable<TimeoutOperator> timeoutOperator;
        [SerializeReference] public BlackboardVariable<float> timeOut;

        public override bool IsTrue()
        {
            return timeoutOperator.Value switch
            {
                TimeoutOperator.GreaterThan => Time.time - lastTime.Value > timeOut.Value,
                TimeoutOperator.LessThan => Time.time - lastTime.Value < timeOut.Value,
                TimeoutOperator.GreaterThanOrEqualTo => Time.time - lastTime.Value >= timeOut.Value,
                TimeoutOperator.LessThanOrEqualTo => Time.time - lastTime.Value <= timeOut.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}