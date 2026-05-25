using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    [CreateAssetMenu(menuName = "Spells/Gravity Surge")]
    public sealed class GravitonSurgeDefinition : SpellDefinition
    {
        [Header("References")]
        public GravityPulseAttractor surgePrefab;

        [Header("Settings")]
        public float activeDuration = 4f;

        public override ISpellRuntime CreateRuntime()
        {
            return new GravitonSurgeRuntime();
        }
    }
}