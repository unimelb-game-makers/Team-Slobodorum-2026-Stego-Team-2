using Unity.Behavior;

namespace TeamSlobodorum.Entities.HostileRobot.Behaviour
{
    [BlackboardEnum]
    public enum EnemyState
    {
        Normal,
        Alert,
        Search,
        Attack,
    }
}