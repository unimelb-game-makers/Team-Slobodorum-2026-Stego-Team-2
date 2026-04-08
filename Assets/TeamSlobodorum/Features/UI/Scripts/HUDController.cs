using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Spells;
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
        private Spellcaster _spellcaster;
        private PlayerEntity _playerEntity;
        
        private Label _hitPointsLabel;
        private Label _currentSpellLabel;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            
            var root = _uiDocument.rootVisualElement;
            _hitPointsLabel = root.Q<Label>("HitPointsLabel");
            _currentSpellLabel = root.Q<Label>("CurrentSpellLabel");
        }
        
        private void Start()
        {
            _cancelAction = InputSystem.actions.FindAction("Cancel");
            
            var playerObject = GameObject.FindWithTag("Player");
            _spellcaster = playerObject.GetComponent<Spellcaster>();
            _playerEntity = playerObject.GetComponent<PlayerEntity>();
            
            _spellcaster.CurrentSpellChanged += UpdateCurrentSpell;
            _playerEntity.Damaged += UpdateHitPoints;
            
            UpdateCurrentSpell(_spellcaster.CurrentSpell);
            UpdateHitPoints();
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            if (_cancelAction.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void UpdateCurrentSpell(Spell spell)
        {
            _currentSpellLabel.text = $"CurrentSpell: {spell.SpellName}";
        }

        private void UpdateHitPoints()
        {
            _hitPointsLabel.text = $"HP: {_playerEntity.HitPoints:F2}";
        }
    }
}
