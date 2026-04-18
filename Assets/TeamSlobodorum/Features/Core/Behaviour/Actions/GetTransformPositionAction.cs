using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Core.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Get Transform Position", story: "Get [position] from [transform]",
        category: "Action/Transform", id: "b8069b7ca673d4975fb86f4d510e55d6")]
    public partial class GetTransformPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> position;
        [SerializeReference] public BlackboardVariable<Transform> transform;

        protected override Status OnStart()
        {
            position.Value = transform.Value.position;
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