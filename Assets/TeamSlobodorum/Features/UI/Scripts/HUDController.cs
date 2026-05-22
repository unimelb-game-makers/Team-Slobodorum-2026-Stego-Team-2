using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private float manaScrollSpeed = 10f;
        
        private UIDocument _uiDocument;

        private InputAction _cancelAction;
        public InputActionReference _attackAction;
        private PlayerSpellCaster _spellcaster;
        private PlayerEntity _playerEntity;
        private PlayerSpellManager _spellManager;
        private ProgressBar _healthBar;
        private VisualElement _healthShadowContainer;
        private VisualElement _manaBar;
        private VisualElement _manaShadowContainer;
        private float _manaBarOffset;

        private VisualElement root;
        private List<Button> equippedSlotButtons;
        private VisualElement _equippedSlotsContainer;
        public Color activateColor;
        public Color deactivateColor;
        private VisualElement _announcementBox;
        private Label _annoucementTitle;
        private Label _annoucementDescription;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;
            _manaBar = root.Q<VisualElement>("ManaBar2");
            _manaShadowContainer = root.Q<VisualElement>("ManaShadowContainer");
            _healthBar = root.Q<ProgressBar>("HealthBar");
            _healthShadowContainer = root.Q<VisualElement>("HealthShadowContainer");
            _announcementBox = root.Q<VisualElement>("Annoucement");
            _annoucementTitle = root.Q<Label>("AnnoucementTitle");
            _annoucementDescription = root.Q<Label>("AnnoucementDescription");
        }

        private void OnEnable()
        {
            _manaBar.schedule.Execute(UpdateManaBackgroundPhysics).Every(0);
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
            _playerEntity.HealthManager.Damaged += _ => UpdateHitPoints();
            _spellcaster.SelectedSpellChanged += UpdateSelectedSpell;

            _spellManager.OnSpellEquipped += RefreshEquippedSlots;
            _spellManager.OnSpellUnequipped += RefreshEquippedSlots;
            _spellManager.OnSpellObtained += RefreshEquippedSlots;

            RefreshEquippedSlots();
            UpdateHitPoints();
            UpdateSelectedSpell();

            Cursor.lockState = CursorLockMode.Locked;

            _spellManager.OnSpellObtained += ShowSpellAnnouncement;
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
            Debug.Log($"CurrentSpell: {_spellcaster.SelectedSpell?.DisplayName}");
            RefreshEquippedSlots();
        }

        private void UpdateHitPoints()
        {
            _healthBar.value = _playerEntity.HealthManager.HitPoints / _playerEntity.HealthManager.maxHitPoints;
            _healthShadowContainer.style.width = Length.Percent(100 * _playerEntity.HealthManager.HitPoints / _playerEntity.HealthManager.maxHitPoints);
        }

        private void UpdateManaPoints()
        {
            var newWidth = Length.Percent(100 * _spellcaster.CurrentMana / _spellcaster.TotalMana);
            _manaBar.style.width = newWidth;
            _manaShadowContainer.style.width = newWidth;
        }
        
        private void UpdateManaBackgroundPhysics(TimerState state)
        {
            _manaBarOffset += manaScrollSpeed * Time.deltaTime;

            // Apply the offset as a pixel length to the background position
            _manaBar.style.backgroundPositionX = new StyleBackgroundPosition(
                new BackgroundPosition(BackgroundPositionKeyword.Left, Length.Percent(_manaBarOffset))
            );
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
                    equippedSlotButtons[i].iconImage =
                        Background.FromTexture2D(_spellManager.EquippedSpells[i].Icon.texture);
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
        public  void showAnnoucement(string title = null, string description = null)
        {
            _announcementBox.style.display = DisplayStyle.Flex;
            _annoucementTitle.text = title ?? _annoucementTitle.text;
            _annoucementDescription.text = description ?? _annoucementDescription.text;
            Sequence announcementSequence = DOTween.Sequence();
            announcementSequence.Append(
                DOTween.To(() => -125f, 
                    x => _announcementBox.style.translate = new StyleTranslate(new Translate(Length.Percent(x), 0)), 
                    0f, 0.6f)
                .SetEase(Ease.OutBack)
            );

            announcementSequence.AppendInterval(5.0f);

            announcementSequence.Append(
                DOTween.To(() => 0f, 
                    x => _announcementBox.style.translate = new StyleTranslate(new Translate(Length.Percent(x), 0)), 
                    -125f, 0.6f)
                .SetEase(Ease.InBack) 
            );

            announcementSequence.OnComplete(() => {
                _announcementBox.style.display = DisplayStyle.None;
            });

        }
        public void ShowSpellAnnouncement(SpellDefinition definition)
        {
            showAnnoucement("New spell collected", "Press tab to open spell menu");

        }
    }

}