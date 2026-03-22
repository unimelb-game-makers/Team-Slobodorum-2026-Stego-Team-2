using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Player
{
    [RequireComponent(typeof(Animator), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float dampTime = 0.2f;
        [SerializeField] private float rotationSpeed = 5f;

        private Animator _animator;
        private Camera _mainCamera;
        private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _sprintAction;

        private float? _targetForward;
        private float? _targetStrafe;
        private Quaternion _targetRotation;

        private static readonly int ForwardKey = Animator.StringToHash("Forward");
        private static readonly int StrafeKey = Animator.StringToHash("Strafe");
        private static readonly int IdleKey = Animator.StringToHash("Idle");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerInput = GetComponent<PlayerInput>();

            _moveAction = _playerInput.actions.FindAction("Move");
            _sprintAction = _playerInput.actions.FindAction("Sprint");
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();
            // print(moveInput);
            if (moveInput != Vector2.zero)
            {
                _animator.SetBool(IdleKey, false);

                var targetDirection = _mainCamera.transform.forward * moveInput.y +
                                      _mainCamera.transform.right * moveInput.x;
                targetDirection.y = 0;

                // if the target direction is outside the 45-degree angle of the forward direction,
                // rotate to the target direction
                if (Vector3.Dot(targetDirection, transform.forward) < Mathf.Cos(45f * Mathf.Deg2Rad))
                {
                    _targetRotation = Quaternion.LookRotation(targetDirection);
                }

                var moveDirection = Quaternion.FromToRotation(transform.forward, targetDirection) * Vector3.forward;
                if (_sprintAction.IsPressed())
                {
                    moveDirection *= 2;
                }

                _targetForward = moveDirection.z;
                _targetStrafe = moveDirection.x;
            }
            else
            {
                _animator.SetBool(IdleKey, true);
            }

            if (_targetForward != null)
            {
                if (Mathf.Approximately(_targetForward.Value, _animator.GetFloat(ForwardKey)))
                {
                    _targetForward = null;
                }
                else
                {
                    _animator.SetFloat(ForwardKey, _targetForward.Value, dampTime, Time.deltaTime);
                }
            }

            if (_targetStrafe != null)
            {
                if (Mathf.Approximately(_targetStrafe.Value, _animator.GetFloat(StrafeKey)))
                {
                    _targetStrafe = null;
                }
                else
                {
                    _animator.SetFloat(StrafeKey, _targetStrafe.Value, dampTime, Time.deltaTime);
                }
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
