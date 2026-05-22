using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Core.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Get Current Time", story: "Get current time [Time]",
        category: "Action/Delay", id: "13c9b4a605de45dab319e73a4e00f496")]
    public partial class GetCurrentTimeAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the current time.")]
        public BlackboardVariable<float> time;

        protected override Status OnStart()
        {
            time.Value = Time.time;
            return Status.Success;
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