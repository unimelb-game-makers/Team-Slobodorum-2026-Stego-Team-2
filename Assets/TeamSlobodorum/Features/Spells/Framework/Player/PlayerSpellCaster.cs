using System.Collections.Generic;
using TeamSlobodorum.Spells;
using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpellCaster : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] private List<SpellDefinition> spellDefinitions = new();

    // Remove later
    [Tooltip("Temporary explicit slot for fireball.")]
    [SerializeField] private int fireballIndex = 0;
    [Tooltip("Temporary explicit slot for grab.")]
    [SerializeField] private int grabIndex = 1;

    [Header("Origins")]
    [Tooltip("Position where the spell is cast.")]
    [SerializeField] private Transform castOrigin;

    [Tooltip("Position where the player center is.")]
    [SerializeField] private Transform playerOrigin;

    [Header("Input")]
    [SerializeField] private InputActionReference castAction;

    private SpellCoordinator _coordinator;
    private Transform _aimTarget;
    private Camera _mainCamera;

    // Temporary
    private SpellHandle _activeGrabHandle;

    private void Awake()
    {
        _coordinator = FindFirstObjectByType<SpellCoordinator>();
        _mainCamera = Camera.main;

        if (castOrigin == null)
            castOrigin = transform;

        _aimTarget = GameObject.Find("Aim Target")?.transform;
    }

    private void OnEnable()
    {
        if (castAction != null)
        {
            castAction.action.performed += OnCastPerformed;
            castAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (castAction != null)
        {
            castAction.action.performed -= OnCastPerformed;
            castAction.action.Disable();
        }
    }

    private void OnCastPerformed(InputAction.CallbackContext context)
    {
        CastGrab();
    }

    public void CastFireball()
    {
        CastSpell(fireballIndex);
    }

    public void CastGrab()
    {
        if (_coordinator == null)
            return;

        if (_activeGrabHandle.Value != 0 &&
            _coordinator.TryGetRuntime(_activeGrabHandle, out var runtime) &&
            !runtime.IsFinished)
        {
            _coordinator.Cancel(_activeGrabHandle, SpellCancelReason.UserCancelled);
            _activeGrabHandle = default;
            return;
        }

        CastSpell(grabIndex);
    }

    private void CastSpell(int index)
    {
        if (_coordinator == null || castOrigin == null || playerOrigin == null)
        {
            Debug.LogWarning("PlayerSpellCaster is missing references.");
            return;
        }

        if (index < 0 || index >= spellDefinitions.Count)
        {
            Debug.LogWarning($"Spell index {index} is out of range.");
            return;
        }

        SpellDefinition definition = spellDefinitions[index];
        if (definition == null)
        {
            Debug.LogWarning($"Spell definition at index {index} is null.");
            return;
        }

        Transform aimOrigin;
        Vector3 aimDirection;

        if (_aimTarget != null)
        {
            aimOrigin = _aimTarget;
            aimDirection = (_aimTarget.position - castOrigin.position).normalized;
        }
        else if (_mainCamera != null)
        {
            aimOrigin = _mainCamera.transform;
            aimDirection = _mainCamera.transform.forward;
        }
        else
        {
            Debug.LogWarning("No aim target and no main camera found.");
            return;
        }

        Debug.Log("Aim Origin: " + aimOrigin);
        var request = new SpellCastRequest(
            definition,
            gameObject,
            castOrigin,
            aimOrigin,
            aimDirection,
            playerOrigin
        );

        if (!_coordinator.TryCast(request, out _activeGrabHandle))
        {
            Debug.LogWarning($"Failed to cast spell at index {index}.");
            return;
        }

        Debug.Log($"Cast spell '{definition.name}'. Handle = {_activeGrabHandle.Value}");
    }
}