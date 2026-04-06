using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public sealed class SpellContext
    {
        public GameObject Caster { get; }
        public Transform CastOrigin { get; }
        public Vector3 AimOrigin { get; }
        public Vector3 AimDirection { get; }

        public ISpellCoordinator Coordinator { get; }
        public ITargetingService Targeting { get; }
        public IMovementInfluenceService Movement { get; }
        public ISpellEventBus EventBus { get; }

        public SpellContext(
            GameObject caster,
            Transform castOrigin,
            Vector3 aimOrigin,
            Vector3 aimDirection,
            ISpellCoordinator coordinator,
            ITargetingService targeting,
            IMovementInfluenceService movement,
            ISpellEventBus eventBus)
        {
            Caster = caster;
            CastOrigin = castOrigin;
            AimOrigin = aimOrigin;
            AimDirection = aimDirection;
            Coordinator = coordinator;
            Targeting = targeting;
            Movement = movement;
            EventBus = eventBus;
        }


    }
}