using TeamSlobodorum.Spells.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using TeamSlobodorum.Spells;


public class PlayerSpellCaster : MonoBehaviour
{
    [Header("Spell")]
    [SerializeField] private SpellDefinition fireballDefinition;

    [Header("Origins")]
    [Tooltip("Optional. If left empty, this transform is used.")]
    [SerializeField] private Transform castOrigin;

    [Header("Input")]
    [SerializeField] private InputActionReference castAction;

    private SpellCoordinator _coordinator;
    private Transform _aimTarget;
    private Camera _mainCamera;

    private void Awake()
    {
        _coordinator = FindFirstObjectByType<SpellCoordinator>();

        _mainCamera = Camera.main;

        if (castOrigin == null)
            castOrigin = transform;

        var aim = transform.Find("AimTarget");

        _aimTarget = aim;
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
        CastFireball();
    }

    public void CastFireball()
    {
        if (_coordinator == null || fireballDefinition == null || castOrigin == null)
        {
            Debug.LogWarning("PlayerSpellCaster is missing references.");
            return;
        }

        Vector3 aimOrigin;
        Vector3 aimDirection;

        if (_aimTarget != null)
        {
            aimOrigin = _aimTarget.position;
            aimDirection = (_aimTarget.position - castOrigin.position).normalized;
        }
        else
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("No aimTarget and no main camera found.");
                return;
            }

            aimOrigin = cam.transform.position;
            aimDirection = cam.transform.forward;
        }

        var request = new SpellCastRequest(
            fireballDefinition,
            gameObject,
            castOrigin,
            aimOrigin,
            aimDirection
        );

        if (!_coordinator.TryCast(request, out var handle))
        {
            Debug.LogWarning("Failed to cast fireball.");
            return;
        }

        Debug.Log($"Fireball cast. Handle = {handle.Value}");
    }
}
