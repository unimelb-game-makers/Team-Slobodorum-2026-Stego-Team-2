using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start Moving Until Can Attack",
        story: "[Entity] starts moving to a position where it can attack [Target]",
        category: "Action/Movement", id: "632dd0264e9c4010b49e0c97f241697d")]
    public partial class StartMovingUntilCanAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<Entity> entity;
        [SerializeReference] public BlackboardVariable<Entity> target;

        private const float UpdateInterval = 0.1f;

        private IAttackable _attackable;
        private Movement _movement;
        private float _counter;

        protected override Status OnStart()
        {
            if (!entity.Value.TryGetComponent(out _attackable) || !entity.Value.TryGetComponent(out _movement))
            {
                return Status.Failure;
            }

            _movement.StartMovingTo(target.Value.transform.position, _attackable.AttackRange);
            _counter = UpdateInterval;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _counter -= Time.deltaTime;

            if (_movement.IsMoving)
            {
                if (_counter <= 0)
                {
                    _movement.StartMovingTo(target.Value.transform.position, _attackable.AttackRange);
                    _counter = UpdateInterval;
                }

                return Status.Running;
            }

            return _movement.LastMoveSucceeded ? Status.Success : Status.Failure;
        }

        protected override void OnEnd()
        {
        }
    }
}