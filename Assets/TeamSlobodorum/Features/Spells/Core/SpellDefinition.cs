using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public abstract class SpellDefinition : ScriptableObject
    {

        [SerializeField, HideInInspector] private string _spellID;
        [Header("Identity")]
        public SpellId Id;
        public string DisplayName;

        [Header("UI & Presentation")]
        [TextArea(3, 5)]
        public string Description;
        public Sprite Icon;

        [Header("Costs")]
        public int ManaCost = 10;

        [Header("Classification")]
        public SpellCategory Category;
        public SpellTags Tags;

        [Header("Timing")]
        public float CastTime = 0f;
        public float Cooldown = 0f;
        public float RecoveryTime = 0f;

        [Header("Resource Occupancy")]
        public SpellResourceChannel OccupiedChannels;

        public virtual bool RetainHandleAfterCast => false;

        public abstract ISpellRuntime CreateRuntime();


        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_spellID))
            {
                _spellID = System.Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}