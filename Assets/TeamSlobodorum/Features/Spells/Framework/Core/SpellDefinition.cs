using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public abstract class SpellDefinition : ScriptableObject
    {
        [Header("Identity")]
        public SpellId Id;
        public string DisplayName;

        [Header("Classification")]
        public SpellCategory Category;
        public SpellTags Tags;

        [Header("Timing")]
        public float CastTime = 0f;
        public float Cooldown = 0f;
        public float RecoveryTime = 0f;

        [Header("Resource Occupancy")]
        public SpellResourceChannel OccupiedChannels;

        public abstract ISpellRuntime CreateRuntime();
    }
}