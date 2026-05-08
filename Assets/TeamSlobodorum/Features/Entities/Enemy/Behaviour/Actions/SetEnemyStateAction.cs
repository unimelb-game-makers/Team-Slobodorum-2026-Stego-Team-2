using System;
using TeamSlobodorum.Entities.HostileRobot.Behaviour;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Entities.Enemy.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Enemy State", story: "Set enemy state of [EnemyEntity] to [EnemyState]",
        category: "Action/Entity", id: "b927a79026704947a7ebb78fea5d5b6a")]
    public partial class SetEnemyStateAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<EnemyEntity> enemyEntity;
        public BlackboardVariable<EnemyState> enemyState;

        protected override Status OnStart()
        {
            enemyEntity.Value.EnemyState = enemyState.Value;
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