using TeamSlobodorum.Entities.HostileRobot.Behaviour;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using Action = System.Action;

namespace TeamSlobodorum.Entities.Enemy
{
    public abstract class EnemyEntity : LivingEntity
    {
        public event Action EnemyStateChanged;

        public BehaviorGraphAgent BehaviorGraphAgent { get; private set; }

        private SerializableGUID _enemyStateKey;

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
            BehaviorGraphAgent.GetVariableID("EnemyState", out _enemyStateKey);
        }
    }
}