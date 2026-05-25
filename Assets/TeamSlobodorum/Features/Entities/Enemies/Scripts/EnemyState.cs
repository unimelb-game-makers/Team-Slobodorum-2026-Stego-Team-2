using Unity.Behavior;

namespace TeamSlobodorum.Entities.Enemies
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