namespace TeamSlobodorum.Entities
{
    public interface IAttackable
    {
        /// <summary>
        /// The maximum attack range
        /// </summary>
        float AttackRange { get; }

        /// The implementer of this method will determine the type of attack (e.g. melee or ranged)
        /// based on the target (e.g. the distance to it)
        void Attack(Entity target);
    }
}
