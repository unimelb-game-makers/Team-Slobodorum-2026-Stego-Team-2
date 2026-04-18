using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public sealed class SpellContext
    {
        public GameObject Caster { get; }
        public Transform CastOrigin { get; }
        public Transform AimOrigin { get; }
        public Vector3 AimDirection { get; }
        public Transform PlayerOrigin { get; }

        public ISpellCoordinator Coordinator { get; }
        public ITargetingService Targeting { get; }
        public IMotionService Movement { get; }
        public ISpellEventBus EventBus { get; }

        public SpellContext(
            GameObject caster,
            Transform castOrigin,
            Transform aimOrigin,
            Vector3 aimDirection,
            Transform playerOrigin,
            ISpellCoordinator coordinator,
            ITargetingService targeting,
            IMotionService movement,
            ISpellEventBus eventBus)
        {
            Caster = caster;
            CastOrigin = castOrigin;
            AimOrigin = aimOrigin;
            AimDirection = aimDirection;
            PlayerOrigin = playerOrigin;
            Coordinator = coordinator;
            Targeting = targeting;
            Movement = movement;
            EventBus = eventBus;
        }


    }
}