using TeamSlobodorum.Particles;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Entities;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public sealed class GrabRuntime : SpellRuntimeBase
    {
        private Entity _targetEntity;
        private Movement _targetMovement;
        private bool _useGravity;
        private float _grabDistance;

        private LinkEffect _linkEffect;
        private GrabDefinition def => (GrabDefinition)Definition;

        public override bool IsFinished =>
            State == SpellExecutionState.Completed ||
            State == SpellExecutionState.Cancelled;

        public override void Begin(SpellContext context)
        {
            base.Begin(context);


            if (context.CastOrigin == null || context.Targeting == null || !context.Targeting.TryResolveEntity(context.AimOrigin.position, out _targetEntity))
            {
                if (_targetEntity == null) Debug.Log("Entity not found!");
                State = SpellExecutionState.Cancelled;
                return;
            }

            if (_targetEntity.Rigidbody == null)
            {
                State = SpellExecutionState.Cancelled;
                return;
            }

            _useGravity = _targetEntity.Rigidbody.useGravity;
            _targetEntity.Rigidbody.useGravity = false;
            _grabDistance = def.preserveInitialDistance ? 
                Vector3.Distance(_targetEntity.Rigidbody.transform.position, context.AimOrigin.position) : 
                def.fixedHoldDistance;

            if (_targetEntity.TryGetComponent(out _targetMovement))
            {
                _targetMovement.PreventMovement = true;
            }

            if (def.linkEffectPrefab != null && context.CastOrigin != null)
            {
                _linkEffect = Object.Instantiate(def.linkEffectPrefab, context.CastOrigin);
                _linkEffect.endPoint = _targetEntity.transform;
                _linkEffect.gameObject.SetActive(true);
            }

            State = SpellExecutionState.Active;
        }

        public override void FixedTick(float deltaTime)
        {
            if (State != SpellExecutionState.Active)
                return;

            var targetPos = Context.PlayerOrigin.position + Vector3.Normalize(Context.AimOrigin.position - Context.PlayerOrigin.position) * _grabDistance;
            var direction = targetPos - _targetEntity.Rigidbody.position;

            if (direction.sqrMagnitude < 0.0001f)
            {
                _targetEntity.Rigidbody.linearVelocity = Vector3.zero;
                return;
            }

            float speed = Mathf.Min(direction.magnitude * def.pullGain, def.maxSpeed);
            _targetEntity.Rigidbody.linearVelocity = direction.normalized * speed;
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
            if (_targetEntity != null && _targetEntity.Rigidbody != null)
            {
                _targetEntity.Rigidbody.useGravity = _useGravity;
            }
            
            if (_targetMovement != null)
            {
                _targetMovement.PreventMovement = false;
            }

            if (_linkEffect != null)
            {
                Object.Destroy(_linkEffect.gameObject);
            }

            _targetEntity = null;
            _linkEffect = null;
        }
    }
}