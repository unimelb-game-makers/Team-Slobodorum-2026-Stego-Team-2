using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    [CreateAssetMenu(menuName = "Spells/Explosion")]
    public sealed class ExplosionDefinition : SpellDefinition
    {
        [Header("References")]
        public GameObject explosionVfxPrefab;

        [Header("Settings")]
        public float radius = 5f;
        public float force = 20f;
        public float upwardsModifier = 0.25f;

        [Header("Filtering")]
        public LayerMask affectedLayers = ~0;
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;
        public bool affectCaster = false;
        public int maxColliders = 128;

        public override ISpellRuntime CreateRuntime()
        {
            return new ExplosionRuntime();
        }
    }
}