using UnityEngine;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells;



[CreateAssetMenu(menuName = "Spells/Fireball")]
    public class FireballDefinition : SpellDefinition
    {
        public GameObject ProjectilePrefab;
        public float SpeedRate = 1f;
        public float Lifetime = 15f;

        public override ISpellRuntime CreateRuntime()
        {
            return new FireballRuntime();
        }
    }
