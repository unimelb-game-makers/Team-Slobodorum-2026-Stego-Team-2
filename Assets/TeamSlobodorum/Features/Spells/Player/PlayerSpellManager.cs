using System;
using System.Collections.Generic;
using System.Linq;
using TeamSlobodorum.Spells.Collectibles;
using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Spells.Player
{
    public class PlayerSpellManager : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private List<SpellDefinition> obtainedSpells = new();
        [SerializeField] private List<SpellDefinition> equippedSpells = new();

        public IReadOnlyList<SpellDefinition> ObtainedSpells => obtainedSpells;
        public IReadOnlyList<SpellDefinition> EquippedSpells => equippedSpells;
        public event Action<SpellDefinition> OnSpellObtained;
        public event Action<SpellDefinition> OnSpellEquipped;
        public event Action<SpellDefinition> OnSpellUnequipped;

        public InputActionReference pickupAction;
        [HideInInspector] public List<SpellCollectibles> collectibles = new List<SpellCollectibles>();

        public void Start()
        {
            pickupAction.action.performed += TryPickupSpell;
        }

        private void TryPickupSpell(InputAction.CallbackContext context)
        {
            if (collectibles.Count != 0)
            {
                if (ObtainSpell(collectibles[0].SpellDefinition))
                {
                    collectibles[0].Collected();
                    collectibles.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Adds a spell to the player's permanent collection.
        /// </summary>
        public bool ObtainSpell(SpellDefinition newSpell)
        {
            if (newSpell == null) return false;
            
            if (obtainedSpells.Contains(newSpell))
            {
                Debug.LogWarning($"Player already obtained spell: {newSpell.name}");
                return true;
            }

            obtainedSpells.Add(newSpell);
            TryEquipSpell(newSpell);
            
            OnSpellObtained?.Invoke(newSpell);
            return true;
        }

        /// <summary>
        /// Attempts to equip a spell the player has already obtained.
        /// </summary>
        public bool TryEquipSpell(SpellDefinition spellToEquip)
        {
            if (spellToEquip == null) return false;
            if (!obtainedSpells.Contains(spellToEquip))
            {
                Debug.LogWarning("Cannot equip a spell that hasn't been obtained!");
                return false;
            }

            if (equippedSpells.Contains(spellToEquip))
            {
                Debug.Log("Spell is already equipped.");
                return false;
            }

            equippedSpells.Add(spellToEquip);
            
            OnSpellEquipped?.Invoke(spellToEquip);
            return true;
        }

        /// <summary>
        /// Removes a spell from the active equipped list.
        /// </summary>
        public bool TryUnequipSpell(SpellDefinition spellToUnequip)
        {
            if (spellToUnequip == null || !equippedSpells.Contains(spellToUnequip)) 
                return false;

            equippedSpells.Remove(spellToUnequip);
            
            OnSpellUnequipped?.Invoke(spellToUnequip);
            return true;
        }


        public void OnDestroy()
        {
            pickupAction.action.performed -= TryPickupSpell;
        }
    }
}