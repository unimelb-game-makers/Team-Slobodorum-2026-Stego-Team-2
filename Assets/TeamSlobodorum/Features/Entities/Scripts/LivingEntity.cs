using System;
using TeamSlobodorum.Damage;
using UnityEngine;

namespace TeamSlobodorum.Entities
{
    public class LivingEntity : Entity, IDamageable
    {
        public float maxHitPoints = 100.0f;
        public float invincibleTime;
        [SerializeField] private float fallThreshold = -500f;

        public event Action Died;
        public event Action Damaged;
        public float HitPoints { get; protected set; }
        public virtual bool IsAlive => HitPoints > 0;

        private Flammable.Flammable _flammable;

        private float _invincibleCounter;

        protected override void Awake()
        {   
            base.Awake();
            // put in awake to ensure it get initialised before call 
            HitPoints = maxHitPoints;
            Died += OnDied;
        }
        
        protected virtual void Start()
        {
            if (TryGetComponent(out _flammable))
            {
                _flammable.StopBurning += OnStopBurning;
            }
        }

        protected virtual void Update()
        {
            if (_invincibleCounter > 0)
            {
                _invincibleCounter -= Time.deltaTime;
            }

            // Void Fall
            if (transform.position.y < fallThreshold && IsAlive)
            {
                HitPoints = 0;
                Damaged?.Invoke();
                Died?.Invoke();
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive || _invincibleCounter > 0) return;

            HitPoints -= damage;
            if (HitPoints < 0)
            {
                HitPoints = 0;
            }

            _invincibleCounter = invincibleTime;
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

        protected virtual void OnDied()
        {
        }

        private void OnStopBurning()
        {
            _flammable.ResetStates();
        }
    }
}