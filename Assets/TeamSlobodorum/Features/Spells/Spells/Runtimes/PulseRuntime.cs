using TeamSlobodorum.Entities;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Spells.Motion;
using TeamSlobodorum.Spells;
using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public class PulseRuntime : ISpellRuntime
    {
        public SpellHandle Handle { get; private set; }
        public SpellDefinition Definition { get; private set; }
        public SpellExecutionState State { get; private set; }
        public bool IsFinished { get; private set; }

        private PulseDefinition _definition;
        private SpellContext _context;

        private Rigidbody _playerRb;
        private PlayerMotionService _playerMotionService;

        private Entity _targetEntity;
        private Rigidbody _targetRb;

        private Vector3 _forceDirection;
        private float _timeRemaining;

        public void Initialize(SpellHandle handle, SpellDefinition definition)
        {
            Handle = handle;
            Definition = definition;
            _definition = (PulseDefinition)definition;
            State = SpellExecutionState.Casting;
            IsFinished = false;
        }

        public void Begin(SpellContext context)
        {
            _context = context;

            if (_definition == null || _context.Caster == null || _context.Targeting == null)
            {
                Finish();
                return;
            }

            _playerRb = _context.Caster.GetComponentInParent<Rigidbody>();
            _playerMotionService = _context.Caster.GetComponentInParent<PlayerMotionService>();

            if (_playerRb == null || _playerMotionService == null)
            {
                Finish();
                return;
            }

            Vector3 aimDir = _context.AimDirection.normalized;
            if (aimDir.sqrMagnitude < 0.0001f)
            {
                Finish();
                return;
            }

            Vector3 aimPoint = _context.AimOrigin.position + aimDir * _definition.MaxTargetDistance;

            if (!_context.Targeting.TryResolveEntity(
                    aimPoint,
                    _definition.AimProbeRadius,
                    _definition.TargetMask,
                    out var targetEntity))
            {
                Finish(); // no objects detected -> no cast
                return;
            }

            _targetEntity = targetEntity;
            _targetRb = _targetEntity.GetComponentInParent<Rigidbody>();

            _forceDirection = aimDir * _definition.DirectionSign;
            _timeRemaining = _definition.Duration;
            State = SpellExecutionState.Active;
        }

        public void Tick(float deltaTime)
        {
        }

        public void FixedTick(float fixedDeltaTime)
        {
            if (IsFinished || State != SpellExecutionState.Active)
                return;

            if (_timeRemaining <= 0f)
            {
                Finish();
                return;
            }

            Vector3 force = _forceDirection * _definition.ForceMagnitude;

            // Dynamic target moves through physics.
            if (_targetRb != null && !_targetRb.isKinematic)
                _targetRb.AddForce(force, ForceMode.Force);

            // Option B: player always takes the opposite consequence.
            float playerMass = Mathf.Max(0.0001f, _playerRb.mass);
            Vector3 playerAcceleration = -force / playerMass;

            _playerMotionService.Submit(
                playerAcceleration,
                fixedDeltaTime,
                _definition.OverrideXZ,
                _definition.OverrideY);

            _timeRemaining -= fixedDeltaTime;

            if (_timeRemaining <= 0f)
                Finish();
        }

        public void Cancel(SpellCancelReason reason)
        {
            Finish();
        }

        public void ReceiveEvent(ISpellEvent evt)
        {
        }

        private void Finish()
        {
            IsFinished = true;
            State = SpellExecutionState.Completed;
        }
    }
}