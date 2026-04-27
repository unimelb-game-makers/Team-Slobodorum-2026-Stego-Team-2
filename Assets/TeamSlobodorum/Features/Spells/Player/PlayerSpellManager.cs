using System;
using System.Collections.Generic;
using TeamSlobodorum.Spells.Collectibles;
using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Spells.Player
{
    public class PlayerSpellManager : MonoBehaviour 
    {

        [Header("State")]
        [SerializeField] private List<SpellDefinition> _obtainedSpells = new();
        [SerializeField] private List<SpellDefinition> _equippedSpells = new();

        [Header("Configuration")]
        [SerializeField, Min(0)] 
        private int maxEquippedSpells = 4; 

        public IReadOnlyList<SpellDefinition> ObtainedSpells => _obtainedSpells;
        public IReadOnlyList<SpellDefinition> EquippedSpells => _equippedSpells;
        public event Action<SpellDefinition> OnSpellObtained;
        public event Action<SpellDefinition> OnSpellEquipped;
        public event Action<SpellDefinition> OnSpellUnequipped;

        public InputActionReference pickupAction;
        [HideInInspector] public SpellCollectibles collectible = null;

        public void Start()
        {
            pickupAction.action.performed += TryPickupSpell;
        }
        private void TryPickupSpell(InputAction.CallbackContext context)
        {
            if (collectible != null) {
                if (ObtainSpell(collectible.SpellDefinition))
                {
                    collectible.Collected();
                    collectible = null;
                }

            }
        }

        /// <summary>
        /// Adds a spell to the player's permanent collection.
        /// </summary>
        public bool ObtainSpell(SpellDefinition newSpell)
        {   
            if (newSpell == null) return false;
            
            if (_obtainedSpells.Contains(newSpell))
            {
                Debug.LogWarning($"Player already obtained spell: {newSpell.name}");
                return true;
            }

            _obtainedSpells.Add(newSpell);
            
            OnSpellObtained?.Invoke(newSpell);
            return true;
        }
        /// <summary>
        /// Attempts to equip a spell the player has already obtained.
        /// </summary>
        public bool TryEquipSpell(SpellDefinition spellToEquip)
        {
            if (spellToEquip == null) return false;
            if (!_obtainedSpells.Contains(spellToEquip))
            {
                Debug.LogWarning("Cannot equip a spell that hasn't been obtained!");
                return false;
            }

            if (_equippedSpells.Contains(spellToEquip))
            {
                Debug.Log("Spell is already equipped.");
                return false;
            }

            if (_equippedSpells.Count >= maxEquippedSpells)
            {
                Debug.LogWarning("Equip slots are full!");
                return false;
            }

            _equippedSpells.Add(spellToEquip);
            
            OnSpellEquipped?.Invoke(spellToEquip);
            return true;
        }

        /// <summary>
        /// Removes a spell from the active equipped list.
        /// </summary>
        public bool TryUnequipSpell(SpellDefinition spellToUnequip)
        {
            if (spellToUnequip == null || !_equippedSpells.Contains(spellToUnequip)) 
                return false;

            _equippedSpells.Remove(spellToUnequip);
            
            OnSpellUnequipped?.Invoke(spellToUnequip);
            return true;
        }
    }
}