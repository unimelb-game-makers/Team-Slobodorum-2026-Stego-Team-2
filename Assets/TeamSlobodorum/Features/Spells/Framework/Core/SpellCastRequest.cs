using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public readonly struct SpellCastRequest
    {
        public readonly SpellDefinition Definition;
        public readonly GameObject Caster;
        public readonly Transform CastOrigin;
        public readonly Vector3 AimOrigin;
        public readonly Vector3 AimDirection;

        public SpellCastRequest(
            SpellDefinition definition,
            GameObject caster,
            Transform castOrigin,
            Vector3 aimOrigin,
            Vector3 aimDirection)
        {
            Definition = definition;
            Caster = caster;
            CastOrigin = castOrigin;
            AimOrigin = aimOrigin;
            AimDirection = aimDirection;
        }
    }
}