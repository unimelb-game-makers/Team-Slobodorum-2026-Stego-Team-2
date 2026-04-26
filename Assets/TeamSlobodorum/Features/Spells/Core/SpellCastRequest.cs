using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public readonly struct SpellCastRequest
    {
        public readonly SpellDefinition Definition;
        public readonly GameObject Caster;
        public readonly Transform CastOrigin;
        public readonly Transform AimOrigin;
        public readonly Vector3 AimDirection;
        public readonly Transform PlayerOrigin;

        public SpellCastRequest(
            SpellDefinition definition,
            GameObject caster,
            Transform castOrigin,
            Transform aimOrigin,
            Vector3 aimDirection,
            Transform playerOrigin)
        {
            Definition = definition;
            Caster = caster;
            CastOrigin = castOrigin;
            AimOrigin = aimOrigin;
            AimDirection = aimDirection;
            PlayerOrigin = playerOrigin;
        }
    }
}