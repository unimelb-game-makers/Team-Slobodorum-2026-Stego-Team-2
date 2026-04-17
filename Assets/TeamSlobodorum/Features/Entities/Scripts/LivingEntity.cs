using System;
using TeamSlobodorum.Damage;

namespace TeamSlobodorum.Entities
{
    public class LivingEntity : Entity, IDamageable
    {
        public event Action Died;
        public event Action Damaged;
        public float MaxHitPoints { get; protected set; }
        public float HitPoints { get; protected set; }
        public virtual bool IsAlive => HitPoints > 0;
        
        private Flammable.Flammable _flammable;

        protected virtual void Start()
        {
            HitPoints = MaxHitPoints;
            
            if (TryGetComponent(out _flammable))
            {
                _flammable.StopBurning += OnStopBurning;
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            
            HitPoints -= damage;
            if (HitPoints < 0)
            {
                HitPoints = 0;
            }
            Damaged?.Invoke();
            
            if (HitPoints == 0)
            {
                Died?.Invoke();
            }
        }

        public void Kill()
        {
            HitPoints = 0;
        }
        
        private void OnStopBurning()
        {
            _flammable.ResetStates();
        }
    }
}