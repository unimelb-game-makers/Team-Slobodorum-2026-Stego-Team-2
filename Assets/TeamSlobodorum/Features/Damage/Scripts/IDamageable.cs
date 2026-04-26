using System;

namespace TeamSlobodorum.Damage
{
    public interface IDamageable
    {
        event Action Died;
        event Action Damaged;
        
        float HitPoints { get; }
        
        bool IsAlive => HitPoints > 0;

        void TakeDamage(float damage);

        void Kill();
    }
}
