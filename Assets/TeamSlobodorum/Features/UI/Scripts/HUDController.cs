using System.Collections.Generic;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace TeamSlobodorum.UI.Scripts
{
    public class HUDController : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private InputAction _cancelAction;
        public InputActionReference _attackAction;
        private PlayerSpellCaster _spellcaster;
        private PlayerEntity _playerEntity;
        private PlayerSpellManager _spellManager;
        private ProgressBar _manaBar;
        private ProgressBar _healthBar;

        private VisualElement root;
        private List<Button> equippedSlotButtons;
        private VisualElement _equippedSlotsContainer;
        public Color activateColor;
        public Color deactivateColor;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;
            _manaBar = root.Q<ProgressBar>("ManaBar");
            _healthBar = root.Q<ProgressBar>("HealthBar");
        }

        private void Start()
        {
            _equippedSlotsContainer = root.Q<VisualElement>("EquippedSlotsContainer");
            equippedSlotButtons = _equippedSlotsContainer.Query<Button>().ToList();

            _cancelAction = InputSystem.actions.FindAction("Cancel");

            var playerObject = GameObject.FindWithTag("Player");

            _playerEntity = playerObject.GetComponent<PlayerEntity>();
            _spellcaster = playerObject.GetComponent<PlayerSpellCaster>();
            _spellManager = playerObject.GetComponent<PlayerSpellManager>();
            _playerEntity.Damaged += UpdateHitPoints;
            _spellcaster.SelectedSpellChanged += UpdateSelectedSpell;

            _spellManager.OnSpellEquipped += RefreshEquippedSlots;
            _spellManager.OnSpellUnequipped += RefreshEquippedSlots;
            _spellManager.OnSpellObtained += RefreshEquippedSlots;

            RefreshEquippedSlots();
            UpdateHitPoints();
            UpdateSelectedSpell();

            Cursor.lockState = CursorLockMode.Locked;

        }

        private void Update()
        {
            if (Mouse.current != null && _attackAction.action.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (_cancelAction.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.None;
            }
            UpdateManaPoints();
        }

        private void UpdateSelectedSpell()
        {
            Debug.Log($"CurrentSpell: {_spellcaster.SelectedSpell?.DisplayName}") ;
            RefreshEquippedSlots();
        }

        private void UpdateHitPoints()
        {
            _healthBar.title = $"{(int)_playerEntity.HitPoints} / {(int)_playerEntity.maxHitPoints}";
            _healthBar.value = _playerEntity.HitPoints / _playerEntity.maxHitPoints;
        }

        private void UpdateManaPoints()
        {
            _manaBar.title = $"{(int)_spellcaster.CurrentMana} / {(int)_spellcaster.TotalMana}";
            _manaBar.value = _spellcaster.CurrentMana / _spellcaster.TotalMana;

        }

        public void HideHUD()
        {
            root.style.display = DisplayStyle.None;
        }

        public void ShowHUD()
        {
            root.style.display = DisplayStyle.Flex;

        }

        private void RefreshEquippedSlots(SpellDefinition changedSpell = null)
        {
            // Loop through the 4 fixed slots
            for (int i = 0; i < 4; i++)
            {
                if (i < _spellManager.EquippedSpells.Count)
                {
                    // If the slot has a spell, display the icon
                    equippedSlotButtons[i].iconImage = Background.FromTexture2D(_spellManager.EquippedSpells[i].Icon.texture);
                    if (_spellManager.EquippedSpells[i] == _spellcaster.SelectedSpell)
                    {
                        _equippedSlotsContainer[i].style.backgroundColor = activateColor;
                    }
                    else
                    {
                        _equippedSlotsContainer[i].style.backgroundColor = deactivateColor;

                    }

                }
                else
                {
                    // If there is no spell, clear the icon (or set to a default placeholder)
                    equippedSlotButtons[i].iconImage = null;
                    _equippedSlotsContainer[i].style.backgroundColor = new Color(0, 0, 0, 0);
                }
            }
        }
        private void OnDestroy()
        {
            if (_spellManager != null)
            {
                _spellManager.OnSpellEquipped -= RefreshEquippedSlots;
                _spellManager.OnSpellUnequipped -= RefreshEquippedSlots;
                _spellManager.OnSpellObtained -= RefreshEquippedSlots;

            } 
            if (_spellcaster != null)
            {
                _spellcaster.SelectedSpellChanged -= UpdateSelectedSpell;
            }

        }
    }

}