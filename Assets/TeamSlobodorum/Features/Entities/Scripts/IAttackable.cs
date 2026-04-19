namespace TeamSlobodorum.Entities
{
    public interface IAttackable
    {
        float MeleeAttackRange { get; }

        void Attack();
    }
}
