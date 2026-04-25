using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public abstract class PulseDefinition : SpellDefinition
    {
        [Header("Targeting")]
        [SerializeField] private float aimProbeRadius = 0.75f;
        [SerializeField] private float maxTargetDistance = 20f;
        [SerializeField] private LayerMask targetMask = ~0;

        [Header("Force Exchange")]
        [SerializeField] private float forceMagnitude = 45f;
        [SerializeField] private float duration = 0.5f;

        // Used only for player-side acceleration submission.
        // The target uses Rigidbody force directly.
        [SerializeField] private bool overrideXZ = false;
        [SerializeField] private bool overrideY = false;

        public float AimProbeRadius => aimProbeRadius;
        public float MaxTargetDistance => maxTargetDistance;
        public LayerMask TargetMask => targetMask;
        public float ForceMagnitude => forceMagnitude;
        public float Duration => duration;
        public bool OverrideXZ => overrideXZ;
        public bool OverrideY => overrideY;

        // +1 => push, -1 => pull
        public abstract float DirectionSign { get; }

        public override ISpellRuntime CreateRuntime()
        {
            return new PulseRuntime();
        }
    }

    [CreateAssetMenu(menuName = "TeamSlobodorum/Spells/Impulse")]
    public class ImpulseDefinition : PulseDefinition
    {
        public override float DirectionSign => 1f;
    }

    [CreateAssetMenu(menuName = "TeamSlobodorum/Spells/Repulse")]
    public class RepulseDefinition : PulseDefinition
    {
        public override float DirectionSign => -1f;
    }
}