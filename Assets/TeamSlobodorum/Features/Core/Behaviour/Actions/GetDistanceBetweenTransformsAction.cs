using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Core.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Get Distance Between Transforms", story: "Get [Distance] between [Transform1] and [Transform2]",
        category: "Action/Transform", id: "0f01917a47ac46ef904675cde6036a17")]
    public partial class GetDistanceBetweenTransformsAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the distance.")]
        public BlackboardVariable<float> distance;

        [SerializeReference] public BlackboardVariable<Transform> transform1;
        [SerializeReference] public BlackboardVariable<Transform> transform2;

        protected override Status OnStart()
        {
            distance.Value = Vector3.Distance(transform1.Value.position, transform2.Value.position);
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