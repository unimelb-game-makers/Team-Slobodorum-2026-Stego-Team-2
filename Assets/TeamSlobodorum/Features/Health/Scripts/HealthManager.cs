using System;
using UnityEngine;

namespace TeamSlobodorum.Health
{
    public class HealthManager : MonoBehaviour
    {
        public float maxHitPoints = 100.0f;

        private float _immuneToPhysicalAttackTime;

        public event Action Died;
        public event Action<DamageType> Damaged;

        public float HitPoints { get; set; }
        public bool IsAlive => HitPoints > 0;

        protected virtual void Awake()
        {
            HitPoints = maxHitPoints;
        }

        protected virtual void Update()
        {
            if (_immuneToPhysicalAttackTime > 0)
            {
                _immuneToPhysicalAttackTime -= Time.deltaTime;
            }
        }

        public virtual void TakeDamage(float damage, DamageType damageType)
        {
            if (!IsAlive) return;

            if (damageType == DamageType.Physical)
            {
                if (_immuneToPhysicalAttackTime > 0)
                {
                    return;
                }
                _immuneToPhysicalAttackTime = 1f;
            }

            HitPoints -= damage;
            if (HitPoints < 0)
            {
                HitPoints = 0;
            }

            Damaged?.Invoke(damageType);

            if (HitPoints == 0)
            {
                Died?.Invoke();
            }
        }

        public virtual void Kill()
        {
            if (!IsAlive) return;

            HitPoints = 0;
            Damaged?.Invoke(DamageType.Void);
            Died?.Invoke();
        }
    }
}