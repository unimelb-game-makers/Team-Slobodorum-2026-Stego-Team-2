using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Enemies.Behaviour.Actions
{
    [BlackboardEnum]
    public enum AlertProgressOperation
    {
        Increase,
        Decrease,
    }

    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Increase Alert Progress", story: "[Operation] [AlertProgress] by [Distance]",
        category: "Action/Entity", id: "b0581594482a48d799098505376198f2")]
    public partial class IncreaseAlertProgressAction : Action
    {
        [SerializeReference] public BlackboardVariable<AlertProgressOperation> operation;
        [SerializeReference] public BlackboardVariable<float> alertProgress;
        [SerializeReference] public BlackboardVariable<float> distance;

        private const float MinDistance = 0f;
        private const float MaxDistance = 20f;
        private const float MaxAlertSpeed = 100f;
        
        protected override Status OnStart()
        {
            if (operation.Value == AlertProgressOperation.Increase)
            {
                alertProgress.Value += IncreaseRate(distance) * Time.deltaTime;
            }
            else
            {
                alertProgress.Value -= 10 * Time.deltaTime;
            }
            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }

        private static float IncreaseRate(float distance)
        {
            var t = 1f - Mathf.InverseLerp(MinDistance, MaxDistance, distance);
            return MaxAlertSpeed * t;
        }
    }
}