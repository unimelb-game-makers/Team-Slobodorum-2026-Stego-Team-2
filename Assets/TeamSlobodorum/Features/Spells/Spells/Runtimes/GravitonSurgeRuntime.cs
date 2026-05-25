using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public sealed class GravitonSurgeRuntime : SpellRuntimeBase
    {
        private GravityPulseAttractor _spawned;
        private float _remainingTime;

        private GravitonSurgeDefinition def => (GravitonSurgeDefinition)Definition;

        public override bool IsFinished =>
            State == SpellExecutionState.Completed ||
            State == SpellExecutionState.Cancelled;

        public override void Begin(SpellContext context)
        {
            base.Begin(context);

            if (def == null || def.surgePrefab == null)
            {
                Debug.LogWarning("Gravity Surge definition or prefab is missing.");
                State = SpellExecutionState.Cancelled;
                return;
            }

            if (context.AimOrigin == null)
            {
                Debug.LogWarning("Gravity Surge requires an aim origin.");
                State = SpellExecutionState.Cancelled;
                return;
            }

            Vector3 spawnPoint = context.AimOrigin.position;

            _spawned = Object.Instantiate(def.surgePrefab, spawnPoint, Quaternion.identity);

            if (_spawned == null)
            {
                State = SpellExecutionState.Cancelled;
                return;
            }

            if (context.PlayerOrigin != null)
                _spawned.Initialize(context.PlayerOrigin);

            _remainingTime = def.activeDuration;
            State = SpellExecutionState.Active;
        }

        public override void Tick(float deltaTime)
        {
            if (State != SpellExecutionState.Active)
                return;

            if (_spawned == null)
            {
                State = SpellExecutionState.Completed;
                return;
            }

            _remainingTime -= deltaTime;
            if (_remainingTime <= 0f)
                Complete();
        }

        public override void Cancel(SpellCancelReason reason)
        {
            Cleanup();
            base.Cancel(reason);
        }

        private void Complete()
        {
            Cleanup();
            State = SpellExecutionState.Completed;
        }

        private void Cleanup()
        {
            if (_spawned != null)
            {
                Object.Destroy(_spawned.gameObject);
            }

            _spawned = null;
        }
    }
}