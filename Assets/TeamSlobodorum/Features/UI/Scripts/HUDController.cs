using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private UIDocument _uiDocument;
        private UIDocumentBlur _uiBlur;

        [SerializeField] private float manaScrollSpeed = 10f;
        [SerializeField] private VisualTreeAsset spellSlotTemplate;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference attackAction;
        
        private PlayerSpellCaster _spellcaster;
        private PlayerEntity _playerEntity;
        private PlayerSpellManager _spellManager;
        private ProgressBar _healthBar;
        private VisualElement _healthShadowContainer;
        private VisualElement _manaBar;
        private VisualElement _manaShadowContainer;
        private float _manaBarOffset;

        private VisualElement _root;
        private VisualElement _spellsContainer;
        private VisualElement _announcementBox;
        private Label _toastTitle;
        private Label _toastDescription;
        
        private int _selectedSlotIndex;
        private Coroutine _activeTask;
        private Action _cleanupFn;
        private const float TimeBeforeHideCandidates = 5f;
        private const float DefaultDuration = 300f;
        private bool _isCandidatesShowing;
        private float _hideCounter;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _uiBlur = GetComponent<UIDocumentBlur>();

            _root = _uiDocument.rootVisualElement;
            _manaBar = _root.Q<VisualElement>("ManaBar2");
            _manaShadowContainer = _root.Q<VisualElement>("ManaShadowContainer");
            _healthBar = _root.Q<ProgressBar>("HealthBar");
            _healthShadowContainer = _root.Q<VisualElement>("HealthShadowContainer");
            _announcementBox = _root.Q<VisualElement>("Annoucement");
            _toastTitle = _root.Q<Label>("AnnoucementTitle");
            _toastDescription = _root.Q<Label>("AnnoucementDescription");
            _spellsContainer = _root.Q<VisualElement>("Spells");
        }

        private void Start()
        {
            var playerObject = GameObject.FindWithTag("Player");

            _playerEntity = playerObject.GetComponent<PlayerEntity>();
            _spellcaster = playerObject.GetComponent<PlayerSpellCaster>();
            _spellManager = playerObject.GetComponent<PlayerSpellManager>();
            _playerEntity.HealthManager.Damaged += _ => UpdateHitPoints();
            _spellcaster.BeforeSelectPreviousSpell += OnBeforeSelectPreviousSpell;
            _spellcaster.BeforeSelectNextSpell += OnBeforeSelectNextSpell;

            _spellManager.OnSpellEquipped += RefreshEquippedSlots;
            _spellManager.OnSpellUnequipped += RefreshEquippedSlots;
            _spellManager.OnSpellObtained += RefreshEquippedSlots;

            RefreshEquippedSlots();
            UpdateHitPoints();

            Cursor.lockState = CursorLockMode.Locked;

            _spellManager.OnSpellObtained += ShowSpellAnnouncement;
        }
        
        private void OnEnable()
        {
            _manaBar.schedule.Execute(UpdateManaBackgroundPhysics).Every(0);
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
                _spellcaster.BeforeSelectPreviousSpell -= OnBeforeSelectPreviousSpell;
                _spellcaster.BeforeSelectNextSpell -= OnBeforeSelectNextSpell;
            }
        }

        private void Update()
        {
            if (Mouse.current != null && attackAction.action.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (cancelAction.action.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (_hideCounter > 0)
            {
                _hideCounter -= Time.deltaTime;
            }
            else if (_isCandidatesShowing)
            {
                HideSpellCandidates();
            }

            UpdateManaPoints();
        }

        private void OnBeforeSelectPreviousSpell()
        {
            ShowSpellCandidates();
            _hideCounter = TimeBeforeHideCandidates; 

            if (_activeTask != null)
            {
                StopCoroutine(_activeTask);
                _cleanupFn();
            }
            
            var previousNewSpell = _spellcaster.GetPreviousNEquippedSpells(1).Last();
            _activeTask = StartCoroutine(AsyncSelectPreviousSpell(previousNewSpell));
        }
        
        private IEnumerator AsyncSelectPreviousSpell(SpellDefinition previousNewSpell)
        {
            if (_spellsContainer.childCount > 1)
            {
                if (_selectedSlotIndex == 0)
                {
                    _spellsContainer.style.transitionDuration = new List<TimeValue> { new(0) };
                    
                    var newSpellSlot = CreateSpellSlot(previousNewSpell);
                    newSpellSlot.AddToClassList("candidate");
                    _spellsContainer.Insert(0, newSpellSlot);
                    
                    var newX1 = _spellsContainer.style.translate.value.x.value - 100;
                    _spellsContainer.style.translate = new Translate(newX1, 0, 0);
                    _selectedSlotIndex = 1;
                    
                    _spellsContainer.style.transitionDuration = new List<TimeValue> { new(DefaultDuration, TimeUnit.Millisecond) };
                }
                
                _spellsContainer[_selectedSlotIndex - 1].RemoveFromClassList("candidate");
                _spellsContainer[_selectedSlotIndex - 1].RemoveFromClassList("previous");
                _spellsContainer[_selectedSlotIndex - 1].AddToClassList("selected");
                _spellsContainer[_selectedSlotIndex].AddToClassList("candidate");
                _spellsContainer[_selectedSlotIndex].RemoveFromClassList("selected");
                
                _spellsContainer.RemoveAt(_spellsContainer.childCount - 1);
                
                var newX = _spellsContainer.style.translate.value.x.value + 100;
                _spellsContainer.style.translate = new Translate(newX, 0, 0);

                _selectedSlotIndex--;
                _uiBlur.RefreshPanels();

                yield return null;
            }

            _activeTask = null;
        }

        private void OnBeforeSelectNextSpell()
        {
            ShowSpellCandidates();
            _hideCounter = TimeBeforeHideCandidates;
            
            if (_activeTask != null)
            {
                StopCoroutine(_activeTask);
                _cleanupFn();
            }
            
            var nextThreeSpells = _spellcaster.GetNextNEquippedSpells(3);
            var nextNewSpell = nextThreeSpells.Count > 1 ? nextThreeSpells.Last() : _spellcaster.SelectedSpell;
            _activeTask = StartCoroutine(AsyncSelectNextSpell(nextNewSpell));
        }

        private IEnumerator AsyncSelectNextSpell(SpellDefinition nextNewSpell)
        {
            if (_spellsContainer.childCount > 1)
            {
                _spellsContainer[_selectedSlotIndex].AddToClassList("candidate");
                _spellsContainer[_selectedSlotIndex].RemoveFromClassList("selected");
                _spellsContainer[_selectedSlotIndex + 1].RemoveFromClassList("candidate");
                _spellsContainer[_selectedSlotIndex + 1].AddToClassList("selected");
                
                var newSpellSlot = CreateSpellSlot(nextNewSpell);
                newSpellSlot.AddToClassList("candidate");
                _spellsContainer.Add(newSpellSlot);

                var newX = _spellsContainer.style.translate.value.x.value - 100;
                _spellsContainer.style.translate = new Translate(newX, 0, 0);

                _selectedSlotIndex++;
                _uiBlur.RefreshPanels();

                _cleanupFn = () =>
                {
                    _spellsContainer.style.transitionDuration = new List<TimeValue> { new(0) };

                    _spellsContainer.RemoveAt(0);

                    var newX1 = _spellsContainer.style.translate.value.x.value + 100;
                    _spellsContainer.style.translate = new Translate(newX1, 0, 0);
                    _selectedSlotIndex--;

                    _spellsContainer.style.transitionDuration = new List<TimeValue> { new(DefaultDuration, TimeUnit.Millisecond) };
                    _uiBlur.RefreshPanels();
                };

                yield return new WaitForSeconds(0.3f);
                _cleanupFn();
            }

            _activeTask = null;
        }
        
        private void ShowSpellCandidates()
        {
            for (var i = 0; i < _spellsContainer.childCount; i++)
            {
                if (i == _selectedSlotIndex)
                {
                    _spellsContainer[i].AddToClassList("selected");   
                }
                else
                {
                    _spellsContainer[i].style.opacity = 1f;
                }
            }

            _isCandidatesShowing = true;
        }

        private void HideSpellCandidates()
        {
            for (var i = 0; i < _spellsContainer.childCount; i++)
            {
                if (i == _selectedSlotIndex)
                {
                    _spellsContainer[i].RemoveFromClassList("selected");   
                }
                else
                {
                    _spellsContainer[i].style.opacity = 0f;
                }
            }

            _isCandidatesShowing = false;
        }

        private void UpdateHitPoints()
        {
            _healthBar.value = _playerEntity.HealthManager.HitPoints / _playerEntity.HealthManager.maxHitPoints;
            _healthShadowContainer.style.width = Length.Percent(100 * _playerEntity.HealthManager.HitPoints /
                                                                _playerEntity.HealthManager.maxHitPoints);
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
            _root.style.display = DisplayStyle.None;
        }

        public void ShowHUD()
        {
            _root.style.display = DisplayStyle.Flex;
        }
        
        private void RefreshEquippedSlots(SpellDefinition changedSpell = null)
        {
            _spellsContainer.Clear();

            _selectedSlotIndex = 0;
            _spellsContainer.style.translate = new Translate(0, 0, 0);
            
            var selectedSpell = _spellcaster.SelectedSpell;
            if (selectedSpell != null)
            {
                var newSpellSlot = CreateSpellSlot(selectedSpell);
                _spellsContainer.Add(newSpellSlot);
            }

            var nextThreeSpells = _spellcaster.GetNextNEquippedSpells(3);
            foreach (var spell in nextThreeSpells)
            {
                var newSpellSlot = CreateSpellSlot(spell);
                newSpellSlot.AddToClassList("candidate");
                _spellsContainer.Add(newSpellSlot);
            }

            _hideCounter = TimeBeforeHideCandidates;
            ShowSpellCandidates();
            _uiBlur.RefreshPanels();

            // // Loop through the 4 fixed slots
            // for (int i = 0; i < 4; i++)
            // {
            //     if (i < _spellManager.EquippedSpells.Count)
            //     {
            //         // If the slot has a spell, display the icon
            //         _spellsContainer[i].style.display = DisplayStyle.Flex;
            //         _equippedSlotButtons[i].iconImage =
            //             Background.FromTexture2D(_spellManager.EquippedSpells[i].Icon.texture);
            //         if (_spellManager.EquippedSpells[i] == _spellcaster.SelectedSpell)
            //         {
            //             _spellsContainer[i].AddToClassList("selected");
            //         }
            //         else
            //         {
            //             _spellsContainer[i].RemoveFromClassList("selected");
            //         }
            //     }
            //     else
            //     {
            //         _spellsContainer[i].style.display = DisplayStyle.None;
            //     }
            // }
        }

        private TemplateContainer CreateSpellSlot(SpellDefinition spell)
        {
            var newSpellSlot = spellSlotTemplate.Instantiate();
            var button = newSpellSlot.Q<Button>();
            button.iconImage =
                Background.FromTexture2D(spell.Icon.texture);
            return newSpellSlot;
        }

        public void MakeToastMessage(string title = null, string description = null)
        {
            _announcementBox.style.display = DisplayStyle.Flex;
            _toastTitle.text = title ?? _toastTitle.text;
            _toastDescription.text = description ?? _toastDescription.text;
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

            announcementSequence.OnComplete(() => { _announcementBox.style.display = DisplayStyle.None; });
        }

        public void ShowSpellAnnouncement(SpellDefinition definition)
        {
            MakeToastMessage("New spell collected", "Press tab to open spell menu");
        }
    }
}