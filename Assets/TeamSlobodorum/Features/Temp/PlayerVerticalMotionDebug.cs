using UnityEngine;
using TeamSlobodorum.Entities.Player;

public class PlayerVerticalMotionDebug : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool drawOverlay = true;
    [SerializeField] private bool logSuspiciousFrames = true;
    [SerializeField] private float mismatchThreshold = 0.35f;
    [SerializeField] private float signFlipThreshold = 0.20f;

    private float _prevY;
    private float _prevActualVelY;
    private float _commandedVelY;
    private float _actualVelY;
    private float _actualDeltaY;
    private bool _initialized;
    private int _midAirSignFlips;
    private string _lastWarning = "";

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<PlayerController>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (controller != null)
            controller.PostUpdate += OnControllerPostUpdate;
    }

    private void OnDisable()
    {
        if (controller != null)
            controller.PostUpdate -= OnControllerPostUpdate;
    }

    private void OnControllerPostUpdate(Vector3 localVel, float jumpScale)
    {
        // This is the controller's final Y velocity for the frame
        _commandedVelY = localVel.y;
    }

    private void LateUpdate()
    {
        float y = transform.position.y;

        if (!_initialized)
        {
            _prevY = y;
            _initialized = true;
            return;
        }

        float dt = Mathf.Max(Time.deltaTime, 0.0001f);

        _actualDeltaY = y - _prevY;
        _actualVelY = _actualDeltaY / dt;

        bool grounded = controller != null && controller.IsGrounded();
        bool airborne = !grounded;

        bool midAirSignFlip =
            airborne &&
            Mathf.Abs(_prevActualVelY) > signFlipThreshold &&
            Mathf.Abs(_actualVelY) > signFlipThreshold &&
            Mathf.Sign(_prevActualVelY) != Mathf.Sign(_actualVelY);

        if (midAirSignFlip)
            _midAirSignFlips++;

        float velMismatch = Mathf.Abs(_actualVelY - _commandedVelY);
        bool suspicious = airborne && (midAirSignFlip || velMismatch > mismatchThreshold);

        if (logSuspiciousFrames && suspicious)
        {
            _lastWarning =
                $"[Y DEBUG] frame={Time.frameCount} " +
                $"y={y:F4} dy={_actualDeltaY:F4} " +
                $"actualVelY={_actualVelY:F3} commandedVelY={_commandedVelY:F3} " +
                $"mismatch={velMismatch:F3} grounded={grounded} " +
                $"jumping={(controller != null && controller.IsJumping)} " +
                $"rootMotion={(animator != null && animator.applyRootMotion)}";

            Debug.LogWarning(_lastWarning, this);
        }

        if (grounded)
            _midAirSignFlips = 0;

        _prevY = y;
        _prevActualVelY = _actualVelY;
    }

    private void OnGUI()
    {
        if (!drawOverlay)
            return;

        bool grounded = controller != null && controller.IsGrounded();
        bool jumping = controller != null && controller.IsJumping;
        bool rootMotion = animator != null && animator.applyRootMotion;

        GUILayout.BeginArea(new Rect(20, 20, 540, 220), GUI.skin.box);
        GUILayout.Label("Player Vertical Debug");
        GUILayout.Label($"World Y: {transform.position.y:F4}");
        GUILayout.Label($"Delta Y (frame): {_actualDeltaY:F4}");
        GUILayout.Label($"Actual Vel Y: {_actualVelY:F3}");
        GUILayout.Label($"Controller Vel Y: {_commandedVelY:F3}");
        GUILayout.Label($"Velocity mismatch: {Mathf.Abs(_actualVelY - _commandedVelY):F3}");
        GUILayout.Label($"Grounded: {grounded}");
        GUILayout.Label($"IsJumping: {jumping}");
        GUILayout.Label($"Mid-air sign flips: {_midAirSignFlips}");
        GUILayout.Label($"Animator root motion: {rootMotion}");

        if (!string.IsNullOrEmpty(_lastWarning))
            GUILayout.Label($"Last warning: {_lastWarning}");

        GUILayout.EndArea();
    }
}