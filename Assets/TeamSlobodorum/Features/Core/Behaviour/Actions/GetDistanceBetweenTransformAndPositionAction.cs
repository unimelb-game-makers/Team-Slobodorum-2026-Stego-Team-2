using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Core.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Get Distance Between Transform And Position", story: "Get [Distance] between [Transform] and [Position]",
        category: "Action/Transform", id: "f34871a8316544fea888dcd0286aba76")]
    public partial class GetDistanceBetweenTransformAndPositionAction : Action
    {
        [SerializeReference, Tooltip("[Out Value] The field is assigned with the distance.")]
        public BlackboardVariable<float> distance;

        [SerializeReference] public BlackboardVariable<Transform> transform;
        [SerializeReference] public BlackboardVariable<Vector3> position;

        protected override Status OnStart()
        {
            distance.Value = Vector3.Distance(transform.Value.position, position.Value);
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