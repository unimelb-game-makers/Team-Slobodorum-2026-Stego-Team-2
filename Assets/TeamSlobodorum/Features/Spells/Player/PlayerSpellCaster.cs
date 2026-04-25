using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.InputSystem;
namespace TeamSlobodorum.Spells.Player
{
    [RequireComponent(typeof(PlayerSpellManager))]
    public class PlayerSpellCaster : MonoBehaviour
    {
        private IReadOnlyList<SpellDefinition> spellDefinitions => _spellManager.EquippedSpells;

        [Header("Origins")]
        [Tooltip("Position where the spell is cast.")]
        [SerializeField] private Transform castOrigin;

        [Tooltip("Position where the player center is.")]
        [SerializeField] private Transform playerOrigin;

        [Header("Input")]
        [SerializeField] private InputActionReference castAction;

        [SerializeField] private InputActionReference previousSpellAction;
        [SerializeField] private InputActionReference nextSpellAction;
        [SerializeField] private InputActionReference aimAction;



        public event Action SelectedSpellChanged;

        private SpellCoordinator _coordinator;
        private Transform _aimTarget;
        private Camera _mainCamera;
        private PlayerSpellManager _spellManager;
        private int _selectedSpellIndex = 0;
        [CanBeNull]
        public SpellDefinition SelectedSpell => _selectedSpellIndex < 0 || _selectedSpellIndex >= spellDefinitions.Count
            ? null
            : spellDefinitions[_selectedSpellIndex];

        // Only one tracked spell is allowed at a time.
        private SpellHandle _activeSpellHandle;
        private int _activeSpellIndex = -1;

        private void Awake()
        {
            _spellManager = GetComponent<PlayerSpellManager>();
            _spellManager.OnSpellEquipped += HandleSpellEquipped;
            _spellManager.OnSpellUnequipped += HandleSpellUnequipped;
            _coordinator = FindFirstObjectByType<SpellCoordinator>();

            _mainCamera = Camera.main;

            if (castOrigin == null)
                castOrigin = transform;

            _aimTarget = GameObject.Find("Aim Target")?.transform;

            if (spellDefinitions.Count > 0)
                _selectedSpellIndex = Mathf.Clamp(_selectedSpellIndex, 0, spellDefinitions.Count - 1);
            else
                _selectedSpellIndex = -1;
        }

        private void OnEnable()
        {
            BindAction(castAction, OnCastPerformed);
            BindAction(previousSpellAction, OnPreviousSpellPerformed);
            BindAction(nextSpellAction, OnNextSpellPerformed);
        }

        private void OnDisable()
        {
            UnbindAction(castAction, OnCastPerformed);
            UnbindAction(previousSpellAction, OnPreviousSpellPerformed);
            UnbindAction(nextSpellAction, OnNextSpellPerformed);
        }

        private void BindAction(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null || actionRef.action == null)
                return;

            actionRef.action.performed += callback;
            actionRef.action.Enable();
        }

        private void UnbindAction(InputActionReference actionRef, System.Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null || actionRef.action == null)
                return;

            actionRef.action.performed -= callback;
            actionRef.action.Disable();
        }

        private void OnCastPerformed(InputAction.CallbackContext context)
        {
            CastSelectedSpell();
        }

        private void OnPreviousSpellPerformed(InputAction.CallbackContext context)
        {
            SelectPreviousSpell();
        }

        private void OnNextSpellPerformed(InputAction.CallbackContext context)
        {
            SelectNextSpell();
        }

        private void SelectPreviousSpell()
        {
            if (spellDefinitions.Count == 0)
                return;

            CancelActiveSpellIfAny();

            _selectedSpellIndex--;
            if (_selectedSpellIndex < 0)
                _selectedSpellIndex = spellDefinitions.Count - 1;

            SelectedSpellChanged?.Invoke();
            Debug.Log($"Selected spell: {GetSelectedSpellName()}");
        }

        private void SelectNextSpell()
        {
            if (spellDefinitions.Count == 0)
                return;

            CancelActiveSpellIfAny();

            _selectedSpellIndex++;
            if (_selectedSpellIndex >= spellDefinitions.Count)
                _selectedSpellIndex = 0;

            SelectedSpellChanged?.Invoke();
            Debug.Log($"Selected spell: {GetSelectedSpellName()}");
        }

        public void CastSelectedSpell()
        {
            if (_coordinator == null || castOrigin == null || playerOrigin == null)
            {
                Debug.LogWarning("PlayerSpellCaster is missing references.");
                return;
            }

            if (_selectedSpellIndex < 0 || _selectedSpellIndex >= spellDefinitions.Count)
            {
                Debug.LogWarning("No valid spell is currently selected.");
                return;
            }

            SpellDefinition definition = spellDefinitions[_selectedSpellIndex];
            if (definition == null)
            {
                Debug.LogWarning($"Spell definition at index {_selectedSpellIndex} is null.");
                return;
            }

            // If this tracked spell is already active, clicking again cancels it.
            if (definition.RetainHandleAfterCast &&
                _activeSpellHandle.Value != 0 &&
                _activeSpellIndex == _selectedSpellIndex &&
                _coordinator.TryGetRuntime(_activeSpellHandle, out var activeRuntime) &&
                !activeRuntime.IsFinished)
            {
                _coordinator.Cancel(_activeSpellHandle, SpellCancelReason.UserCancelled);
                ClearActiveSpell();
                Debug.Log($"Cancelled spell '{GetSpellDisplayName(definition)}'.");
                return;
            }

            // Casting a new tracked spell cancels the current tracked spell first.
            CancelActiveSpellIfAny();

            if (!TryBuildAim(out Transform aimOrigin, out Vector3 aimDirection))
            {
                Debug.LogWarning("No aim target and no main camera found.");
                return;
            }

            var request = new SpellCastRequest(
                definition,
                gameObject,
                castOrigin,
                aimOrigin,
                aimDirection,
                playerOrigin
            );

            if (!_coordinator.TryCast(request, out var handle))
            {
                Debug.LogWarning($"Failed to cast spell '{GetSpellDisplayName(definition)}'.");
                return;
            }

            if (definition.RetainHandleAfterCast)
            {
                _activeSpellHandle = handle;
                _activeSpellIndex = _selectedSpellIndex;
            }

            Debug.Log($"Cast spell '{GetSpellDisplayName(definition)}'. Handle = {handle.Value}");
        }

        private bool TryBuildAim(out Transform aimOrigin, out Vector3 aimDirection)
        {
            if (_aimTarget != null && aimAction.action.IsPressed())
            {
                aimOrigin = _aimTarget;
                aimDirection = (_aimTarget.position - castOrigin.position).normalized;
                return aimDirection.sqrMagnitude > 0.0001f;
            }

            if (_mainCamera != null)
            {
                aimOrigin = _mainCamera.transform;
                aimDirection = _mainCamera.transform.forward;
                return aimDirection.sqrMagnitude > 0.0001f;
            }

            aimOrigin = null;
            aimDirection = Vector3.zero;
            return false;
        }

        private void CancelActiveSpellIfAny()
        {
            if (_coordinator == null || _activeSpellHandle.Value == 0)
            {
                ClearActiveSpell();
                return;
            }

            if (_coordinator.TryGetRuntime(_activeSpellHandle, out var runtime) && !runtime.IsFinished)
                _coordinator.Cancel(_activeSpellHandle, SpellCancelReason.UserCancelled);

            ClearActiveSpell();
        }

        private void ClearActiveSpell()
        {
            _activeSpellHandle = default;
            _activeSpellIndex = -1;
        }

        private string GetSelectedSpellName()
        {
            if (_selectedSpellIndex < 0 || _selectedSpellIndex >= spellDefinitions.Count)
                return "None";

            return GetSpellDisplayName(spellDefinitions[_selectedSpellIndex]);
        }

        private static string GetSpellDisplayName(SpellDefinition definition)
        {
            if (definition == null)
                return "Null";

            return string.IsNullOrWhiteSpace(definition.DisplayName)
                ? definition.name
                : definition.DisplayName;
        }
        private void OnDestroy()
        {
            if (_spellManager != null)
            {
                _spellManager.OnSpellEquipped -= HandleSpellEquipped;
                _spellManager.OnSpellUnequipped -= HandleSpellUnequipped;
            }
        }
        private void HandleSpellEquipped(SpellDefinition newSpell)
        {
            // If the player had 0 spells and just equipped one, automatically select it
            if (spellDefinitions.Count == 1)
            {
                _selectedSpellIndex = 0;
                SelectedSpellChanged?.Invoke();
                Debug.Log($"Auto-selected newly equipped spell: {GetSelectedSpellName()}");
            }
        }

        private void HandleSpellUnequipped(SpellDefinition removedSpell)
        {
            // 1. If the spell being unequipped is currently being cast/channeled, cancel it!
            if (SelectedSpell == removedSpell && _activeSpellHandle.Value != 0)
            {
                CancelActiveSpellIfAny();
            }

            // 2. Safely adjust the selected index so it doesn't go out of bounds
            if (spellDefinitions.Count == 0)
            {
                // The player has no spells left
                _selectedSpellIndex = -1;
            }
            else if (_selectedSpellIndex >= spellDefinitions.Count)
            {
                // If we were pointing at the last slot, and it was removed, step back by one
                _selectedSpellIndex = spellDefinitions.Count - 1;
            }

            // 3. Announce that the selection has updated (so the HUD can redraw the icons)
            SelectedSpellChanged?.Invoke();
            Debug.Log($"Spell unequipped. New selection: {GetSelectedSpellName()}");
        }
    }
}
