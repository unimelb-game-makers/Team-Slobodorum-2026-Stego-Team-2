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
        
        private Label _currentSpellLabel;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            
            var root = _uiDocument.rootVisualElement;
            _currentSpellLabel = root.Q<Label>("CurrentSpellLabel");
        }
        
        private void Start()
        {
            _cancelAction = InputSystem.actions.FindAction("Cancel");
            _spellcaster = GameObject.FindGameObjectWithTag("Player").GetComponent<Spellcaster>();
            _spellcaster.CurrentSpellChanged += OnCurrentSpellChanged;
            
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

        private void OnCurrentSpellChanged(Spell spell)
        {
            _currentSpellLabel.text = spell.SpellName;
        }
    }
}
