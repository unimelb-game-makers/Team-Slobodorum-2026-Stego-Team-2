using TeamSlobodorum.AI;
using TeamSlobodorum.Entities.HostileRobot.Behaviour;
using TeamSlobodorum.Health;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;
using Action = System.Action;

namespace TeamSlobodorum.Entities.Enemy
{
    public abstract class EnemyEntity : LivingEntity
    {
        public event Action EnemyStateChanged;

        public BehaviorGraphAgent BehaviorGraphAgent { get; private set; }
        public Brain Brain { get; private set; }

        private SerializableGUID _enemyStateKey;
        private SerializableGUID _searchStartTimeKey;

        public EnemyState EnemyState
        {
            get
            {
                BehaviorGraphAgent.GetVariable(_enemyStateKey, out BlackboardVariable<EnemyState> result);
                return result.Value;
            }
            set
            {
                var prev = EnemyState;
                BehaviorGraphAgent.SetVariableValue(_enemyStateKey, value);
                if (prev != value)
                {
                    EnemyStateChanged?.Invoke();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            BehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
            Brain = GetComponent<Brain>();
            
            BehaviorGraphAgent.GetVariableID("EnemyState", out _enemyStateKey);
            BehaviorGraphAgent.GetVariableID("SearchStartTime", out _searchStartTimeKey);
        }

        protected override void Start()
        {
            base.Start();
            HealthManager.Damaged += OnDamaged;
        }

        private void OnDamaged(DamageType damageType)
        {
            if (EnemyState < EnemyState.Search)
            {
                BehaviorGraphAgent.SetVariableValue(_searchStartTimeKey, Time.time);
                EnemyState = EnemyState.Search;
            }
        }
    }
}