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
        private PlayerSpellCaster _spellcaster;
        private PlayerEntity _playerEntity;

        private Label _hitPointsLabel;
        private Label _currentSpellLabel;
        private VisualElement root;
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;
            _hitPointsLabel = root.Q<Label>("HitPointsLabel");
            _currentSpellLabel = root.Q<Label>("CurrentSpellLabel");
        }

        private void Start()
        {
            _cancelAction = InputSystem.actions.FindAction("Cancel");

            var playerObject = GameObject.FindWithTag("Player");

            _playerEntity = playerObject.GetComponent<PlayerEntity>();
            _spellcaster = playerObject.GetComponent<PlayerSpellCaster>();

            _playerEntity.Damaged += UpdateHitPoints;
            _spellcaster.SelectedSpellChanged += UpdateSelectedSpell;

            UpdateHitPoints();
            UpdateSelectedSpell();

            Cursor.lockState = CursorLockMode.Locked;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnMenuOpened += HideHUD;
                UIManager.Instance.OnMenuClosed += ShowHUD;
            }
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

        private void UpdateSelectedSpell()
        {
            _currentSpellLabel.text = $"CurrentSpell: {_spellcaster.SelectedSpell?.DisplayName}";
        }

        private void UpdateHitPoints()
        {
            _hitPointsLabel.text = $"HP: {_playerEntity.HitPoints:F2}";
        }

        private void OnDestroy()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnMenuOpened -= HideHUD;
                UIManager.Instance.OnMenuClosed -= ShowHUD;
            }
        }

        private void HideHUD()
        {
            root.style.display = DisplayStyle.None;
        }

        private void ShowHUD()
        {
            root.style.display = DisplayStyle.Flex;
        }
    }

}