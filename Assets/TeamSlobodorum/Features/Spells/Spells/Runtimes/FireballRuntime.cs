using TeamSlobodorum.Particles;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public sealed class FireballRuntime : SpellRuntimeBase
    {
        private GameObject _instance;
        private float _timeRemaining;
        private FireballDefinition def => (FireballDefinition)Definition;

        public override bool IsFinished =>
            State == SpellExecutionState.Completed ||
            State == SpellExecutionState.Cancelled;

        public override void Begin(SpellContext context)
        {
            base.Begin(context);

            if (def.ProjectilePrefab == null || context.CastOrigin == null)
            {
                State = SpellExecutionState.Cancelled;
                return;
            }

            Vector3 dir = context.AimDirection.sqrMagnitude > 0.0001f
                ? context.AimDirection.normalized
                : context.CastOrigin.forward;

            _instance = Object.Instantiate(
                def.ProjectilePrefab,
                context.CastOrigin.position,
                Quaternion.LookRotation(dir)
            );

            var projectile = _instance.GetComponent<FireProjectile>();
            if (projectile == null || projectile.Rigidbody == null)
            {
                Object.Destroy(projectile);
                State = SpellExecutionState.Cancelled;
                return;
            }

            projectile.Rigidbody.AddForce(_instance.transform.forward * def.SpeedRate * 10f, ForceMode.Impulse);

            _timeRemaining = def.Lifetime;
            State = SpellExecutionState.Active;
        }

        public override void Tick(float deltaTime)
        {
            if (State != SpellExecutionState.Active)
                return;

            _timeRemaining -= deltaTime;
            if (_timeRemaining <= 0f)
            {
                if (_instance != null)
                    Object.Destroy(_instance);

                State = SpellExecutionState.Completed;
            }
        }

        public override void Cancel(SpellCancelReason reason)
        {
            if (_instance != null)
                Object.Destroy(_instance);

            base.Cancel(reason);
        }
    }
}