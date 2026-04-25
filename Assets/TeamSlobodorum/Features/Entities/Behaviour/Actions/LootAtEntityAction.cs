using System;
using Unity.Behavior;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Look At Entity",
        story:
        "[Entity] looks at [Transform] with [Duration]",
        category: "Action/Entity", id: "9f3e6de3441f4f1ca38f9fcf0130ae23")]
    public partial class LookAtEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<Entity> entity;
        [SerializeReference] public BlackboardVariable<Transform> transform;

        private const float Damping = 0.5f;
        private const float Threshold = 0.1f;

        private float _counter;

        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            var rigidbody = entity.Value.Rigidbody;
            var direction = transform.Value.position - rigidbody.position;
            direction.y = 0;
            direction.Normalize();
            
            if (direction == Vector3.zero)
            {
                return Status.Success;
            }
            
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            if (Quaternion.Angle(rigidbody.rotation, targetRotation) > Threshold)
            {
                rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, targetRotation,
                    Damper.Damp(1, Damping, Time.deltaTime)));
                return Status.Running;
            }

            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}