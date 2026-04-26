using UnityEngine;
using UnityEngine.UIElements;
using TeamSlobodorum.Spells.Player;
using TeamSlobodorum.Spells.Core;
using System.Collections.Generic;
using System.Linq;
namespace TeamSlobodorum.UI.Scripts
{

    public class SpellMenuController : MonoBehaviour
    {
        [Header("Backend Reference")]
        private PlayerSpellManager _spellManager;
        public UIDocument menuDocument;

        // UI Element References
        private VisualElement gridContent; // Container for dynamically generated slots
        private List<Button> equippedSlotButtons; // Cache the 4 fixed equipped slot buttons
        private VisualElement _equippedSlotsContainer;
        private Label detailName;
        private Label detailDescription;
        private VisualElement detailIcon;
        public Color activateColor;
        public Color deactivateColor;
        // State
        private SpellDefinition currentlySelectedSpell;

        private void OnEnable()
        {
            var playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                _spellManager = playerObject.GetComponent<PlayerSpellManager>();
            }

            var root = menuDocument.rootVisualElement;

            // 1. Get core containers
            gridContent = root.Q<VisualElement>("GridContent");
            _equippedSlotsContainer = root.Q<VisualElement>("EquippedSlotsContainer");

            var detailPanel = root.Q<VisualElement>("DetailsPanel");
            detailName = detailPanel.Q<Label>("DetailName");
            detailDescription = detailPanel.Q<Label>("DetailDescription");
            detailIcon = detailPanel.Q<VisualElement>("DetailIcon");

            // 2. Cache the 4 fixed equipped slot buttons and bind Pointer events once
            equippedSlotButtons = _equippedSlotsContainer.Query<Button>().ToList();
            for (int i = 0; i < equippedSlotButtons.Count; i++)
            {
                int slotIndex = i; // Capture local variable for the Lambda expression

                equippedSlotButtons[i].RegisterCallback<PointerDownEvent>(
                        evt => OnEquippedSlotPointerDown(evt, slotIndex),
                        TrickleDown.TrickleDown
                    );
            }

            if (_spellManager != null)
            {
                _spellManager.OnSpellEquipped += RefreshUI;
                _spellManager.OnSpellUnequipped += RefreshUI;
                _spellManager.OnSpellObtained += RefreshUI;
            }

            // 4. Initialize UI state
            ClearDetailsPanel();
            RefreshUI(); // Force an initial refresh on startup to populate data
        }

        private void OnDisable()
        {
            if (_spellManager != null)
            {
                _spellManager.OnSpellEquipped -= RefreshUI;
                _spellManager.OnSpellUnequipped -= RefreshUI;
                _spellManager.OnSpellObtained -= RefreshUI;
            }
        }



        private void OnEquippedSlotPointerDown(PointerDownEvent evt, int slotIndex)
        {
            if (slotIndex >= _spellManager.EquippedSpells.Count) return;

            var spell = _spellManager.EquippedSpells[slotIndex];


            if (evt.button == 0)
            {
                SelectSpellToView(spell);
            }
            else if (evt.button == 1)
            {
                _spellManager.TryUnequipSpell(spell);
            }
        }


        private void OnObtainedSlotPointerDown(PointerDownEvent evt, SpellDefinition spell)
        {
            if (evt.button == 0)
            {
                SelectSpellToView(spell);
            }
            else if (evt.button == 1)
            {
                if (_spellManager.EquippedSpells.Contains(spell))
                {
                    _spellManager.TryUnequipSpell(spell);
                }
                else
                {
                    _spellManager.TryEquipSpell(spell);
                }
                SelectSpellToView(spell);
            }
        }

        // --- Refresh Mechanism ---

        private void RefreshUI(SpellDefinition changedSpell = null)
        {
            RefreshEquippedSlots();
            RefreshObtainedSlots();

            // After refreshing, update the details panel if a spell is currently selected
            if (currentlySelectedSpell != null)
            {
                SelectSpellToView(currentlySelectedSpell);
            }
        }

        private void RefreshEquippedSlots()
        {
            // Loop through the 4 fixed slots
            for (int i = 0; i < 4; i++)
            {
                if (i < _spellManager.EquippedSpells.Count)
                {
                    // If the slot has a spell, display the icon
                    equippedSlotButtons[i].iconImage = Background.FromTexture2D(_spellManager.EquippedSpells[i].Icon.texture);
                    _equippedSlotsContainer[i].style.backgroundColor = activateColor;

                }
                else
                {
                    // If there is no spell, clear the icon (or set to a default placeholder)
                    equippedSlotButtons[i].iconImage = null;
                    _equippedSlotsContainer[i].style.backgroundColor = deactivateColor;
                }
            }
        }

        private void RefreshObtainedSlots()
        {
            // 1. Clear all existing dynamic slots
            gridContent.Clear();

            // 2. Loop through the obtained spells list and dynamically generate slots as needed
            foreach (var spell in _spellManager.ObtainedSpells)
            {
                // Create a new VisualElement as a slot
                VisualElement newSlot = new VisualElement();

                // Add the USS class name you created in UI Builder (controls size and diamond shape)
                newSlot.AddToClassList("spell-obtained-slot");

                // Set the icon
                newSlot.style.backgroundImage = new StyleBackground(spell.Icon);

                newSlot.RegisterCallback<PointerDownEvent>(evt => OnObtainedSlotPointerDown(evt, spell));

                // Add the generated slot to the container
                gridContent.Add(newSlot);
                if (_spellManager.EquippedSpells.Contains(spell))
                {
                    newSlot.style.backgroundColor = activateColor;
                }
                else
                {
                    newSlot.style.backgroundColor = deactivateColor;
                }
            }
        }

        public void SelectSpellToView(SpellDefinition spell)
        {
            currentlySelectedSpell = spell;
            detailName.text = spell.DisplayName;
            detailDescription.text = spell.Description;
            detailIcon.style.backgroundImage = new StyleBackground(spell.Icon);
            if (_spellManager.EquippedSpells.Contains(spell))
            {
                detailIcon.style.backgroundColor = activateColor;
            }
            else
            {
                detailIcon.style.backgroundColor = deactivateColor;
            }

        }

        private void ClearDetailsPanel()
        {
            currentlySelectedSpell = null;
            detailName.text = "";
            detailDescription.text = "";
            detailIcon.style.backgroundImage = null;
            detailIcon.style.backgroundColor = new Color(0, 0, 0, 0);

        }
    }
}