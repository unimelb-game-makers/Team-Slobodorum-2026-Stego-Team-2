using System;
using TeamSlobodorum.Entities.Humanoid;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerMovement : HumanoidMovement
    {
        private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _sprintAction;
        
        public bool IsStrafeMode { get; set; }

        // These are part of a strategy to combat input gimbal lock when controlling an entity
        // that can move freely on surfaces that go upside-down relative to the camera.
        // This is only used in the specific situation where the character is upside-down relative to the input frame,
        // and the input directives become ambiguous.
        // If the camera and input frame are traveling along with the entity, then these are not used.
        private bool _inTopHemisphere = true;
        private float _timeInHemisphere = 100;
        private Vector2 _lastRawInput;
        private const float BlendTime = 2f;
        private Camera _mainCamera;

        private static readonly float RotateThreshold = Mathf.Cos(50f * Mathf.Deg2Rad);
        private static readonly Quaternion UpsideDown = Quaternion.AngleAxis(180, Vector3.left);

        protected override void Awake()
        {
            base.Awake();

            _playerInput = GetComponent<PlayerInput>();

            _moveAction = _playerInput.actions.FindAction("Move");
            _sprintAction = _playerInput.actions.FindAction("Sprint");

            NavMeshAgent.enabled = false;
        }

        protected override void Start()
        {
            base.Start();
            _mainCamera = Camera.main;
        }

        protected override void FixedUpdate()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();

            IsSprinting = _sprintAction.IsPressed();

            if (CanMove)
            {
                if (moveInput != Vector2.zero)
                {
                    IsMoving = true;

                    var inputFrame = GetInputFrame(Vector2.Dot(moveInput, _lastRawInput) < 0.8f);
                    var targetDirection = inputFrame * new Vector3(moveInput.x, 0, moveInput.y);

                    var rotationNeeded =
                        !IsStrafeMode &&
                        (Math.Abs(moveInput.x) < 0.2 ||
                         Vector3.Dot(targetDirection, transform.forward) < RotateThreshold);
                    if (rotationNeeded)
                    {
                        var qA = Rigidbody.rotation;
                        var qB = Quaternion.LookRotation(targetDirection, Vector3.up);
                        Rigidbody.MoveRotation(Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.fixedDeltaTime)));
                    }

                    var desiredVelocity = targetDirection * (IsSprinting ? sprintSpeed : normalSpeed);
                    Rigidbody.linearVelocity =
                        new Vector3(desiredVelocity.x, Rigidbody.linearVelocity.y, desiredVelocity.z);
                }
                else
                {
                    IsMoving = false;
                    Rigidbody.linearVelocity = new Vector3(0, Rigidbody.linearVelocity.y, 0);
                }
            }

            _lastRawInput = moveInput;

            base.FixedUpdate();
        }

        private void OnJump()
        {
            Jump();
        }
        
        // Get the reference frame for the input.
        // The idea is to map camera fwd/right to the entity's XZ plane. There is some complexity here to avoid
        // gimbal lock when the entity is tilted 180 degrees relative to the input frame.
        private Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            // Get the raw input frame, depending on forward mode setting
            var frame = _mainCamera.transform.rotation;

            // Map the raw input frame to something that makes sense as a direction for the entity
            var entityUp = transform.up;
            var up = frame * Vector3.up;

            // Is the entity in the top or bottom hemisphere?  This is needed to avoid gimbal lock,
            // but only when the entity is upside-down relative to the input frame.
            _timeInHemisphere += Time.fixedDeltaTime;
            var inTopHemisphere = Vector3.Dot(up, entityUp) >= 0;
            if (inTopHemisphere != _inTopHemisphere)
            {
                _inTopHemisphere = inTopHemisphere;
                _timeInHemisphere = Mathf.Max(0, BlendTime - _timeInHemisphere);
            }

            // If the entity is untilted relative to the input frame, then early-out with a simple LookRotation
            var axis = Vector3.Cross(up, entityUp);
            if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
            {
                return frame;
            }

            // Entity is tilted relative to input frame: tilt the input frame to match
            var angle = UnityVectorExtensions.SignedAngle(up, entityUp, axis);
            var frameA = Quaternion.AngleAxis(angle, axis) * frame;

            // If the entity is tilted, then we need to get tricky to avoid gimbal-lock
            // when entity is tilted 180 degrees.  There is no perfect solution for this,
            // we need to cheat it :/
            var frameB = frameA;
            if (!inTopHemisphere || _timeInHemisphere < BlendTime)
            {
                // Compute an alternative reference frame for the bottom hemisphere.
                // The two reference frames are incompatible where they meet, especially
                // when entity up is pointing along the X axis of camera frame.
                // There is no one reference frame that works for all entity directions.
                frameB = frame * UpsideDown;
                var axisB = Vector3.Cross(frameB * Vector3.up, entityUp);
                if (axisB.sqrMagnitude > 0.001f)
                    frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
            }

            // Blend timer force-expires when user changes input direction
            if (inputDirectionChanged)
                _timeInHemisphere = BlendTime;

            // If we have been long enough in one hemisphere, then we can just use its reference frame
            if (_timeInHemisphere >= BlendTime)
                return inTopHemisphere ? frameA : frameB;

            // Because frameA and frameB do not join seamlessly when entity Up is along X axis,
            // we blend them over a time in order to avoid degenerate spinning.
            // This will produce weird movements occasionally, but it's the lesser of the evils.
            if (inTopHemisphere)
            {
                return Quaternion.Slerp(frameB, frameA, _timeInHemisphere / BlendTime);
            }

            return Quaternion.Slerp(frameA, frameB, _timeInHemisphere / BlendTime);
        }
    }
}