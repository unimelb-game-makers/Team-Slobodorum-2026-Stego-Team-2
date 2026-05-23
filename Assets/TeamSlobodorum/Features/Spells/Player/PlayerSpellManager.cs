using System;
using System.Collections.Generic;
using System.Linq;
using TeamSlobodorum.DataPersistence;
using TeamSlobodorum.Spells.Collectibles;
using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TeamSlobodorum.Spells.Player
{
    public class PlayerSpellManager : MonoBehaviour, IDataPersistence
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

        public void Awake()
        {
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnSaveRequested += SaveData;
                SaveManager.instance.OnLoadRequested += LoadData;
            }
        }

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

        public void SaveData(GameData data)
        {
            data.spells = _obtainedSpells.Select(spell => new SpellSaveData
            {
                spellID = spell.name,
                isCollected = true,
                isEquipped = _equippedSpells.Contains(spell)
            }).ToList();
        }

        public void LoadData(GameData data)
        {
            _obtainedSpells.Clear();
            _equippedSpells.Clear();
            foreach (SpellSaveData spell in data.spells)
            {

                Addressables.LoadAssetAsync<SpellDefinition>(spell.spellID).Completed += (handle) => OnSpellLoaded(handle, spell); ;
            }
        }
        private void OnSpellLoaded(AsyncOperationHandle<SpellDefinition> handle, SpellSaveData spell)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ObtainSpell(handle.Result);
                if (spell.isEquipped)
                {
                    TryEquipSpell(handle.Result);
                }
            }
            else
            {
                Debug.LogError("Failed to load spell.");
            }
        }



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

            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnSaveRequested -= SaveData;
                SaveManager.instance.OnLoadRequested -= LoadData;
            }

        }
    }
}