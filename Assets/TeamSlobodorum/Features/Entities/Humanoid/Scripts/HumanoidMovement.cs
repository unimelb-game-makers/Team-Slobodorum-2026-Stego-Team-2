using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

namespace TeamSlobodorum.Entities.Humanoid
{
    [RequireComponent(typeof(Rigidbody))]
    public class HumanoidMovement : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Ground speed when walking")]
        public float normalSpeed = 2f;

        [Tooltip("Ground speed when sprinting")]
        public float sprintSpeed = 6f;

        [Tooltip("Initial vertical speed when jumping")]
        public float normalJumpForce = 350f;

        [Tooltip("Initial vertical speed when sprint-jumping")]
        public float sprintJumpForce = 380f;

        [Tooltip("Transition duration (in seconds) when the entity changes velocity or rotation.")]
        public float damping = 0.5f;

        [Header("Animation")]
        [Tooltip("Never speed up the sprint animation more than this, to avoid absurdly fast movement")]
        public float maxSprintScale = 3f;

        [Tooltip("Scale factor for the overall speed of the jump animation")]
        public float jumpBaseScale = 1f;

        public event Action StartMoving;
        public event Action StopMoving;
        public event Action Landed;

        public enum UpModes
        {
            Entity,
            World
        }

        /// <summary>
        /// <para>Up direction for computing motion:</para>
        /// <b>Entity</b>: Move in the Entity's local XZ plane.<br/>
        /// <b>World</b>: Move in global XZ plane.
        /// </summary>
        protected UpModes UpMode = UpModes.World;

        protected struct AnimationParams
        {
            public static readonly int MovingKey = Animator.StringToHash("Moving");
            public static readonly int ForwardKey = Animator.StringToHash("Forward");
            public static readonly int StrafeKey = Animator.StringToHash("Strafe");
            public static readonly int MotionScaleKey = Animator.StringToHash("MotionScale");
            public static readonly int JumpScaleKey = Animator.StringToHash("JumpScale");
            public static readonly int JumpKey = Animator.StringToHash("Jump");
            public static readonly int FallKey = Animator.StringToHash("Fall");
            public static readonly int LandKey = Animator.StringToHash("Land");

            public bool IsRunning;
            public bool JumpTriggered;
            public bool FallTriggered;
            public bool LandTriggered;
            public Vector3 Direction; // normalized direction of motion
            public Vector3 DirectionVelocity;
            public float MotionScale; // scale factor for the animation speed
            public float JumpScale; // scale factor for the jump animation
        }

        protected Rigidbody Rigidbody { get; private set; }
        protected Animator Animator { get; private set; }
        protected NavMeshAgent NavMeshAgent { get; private set; }

        private AnimationParams _animationParams;
        private const float IdleThreshold = 0.2f;

        const float DelayBeforeInferringFall = 0.3f;
        private float _timeLastGrounded;

        public bool IsSprinting { get; set; }
        private bool _isMoving;

        public bool IsMoving
        {
            get => _isMoving;
            protected set
            {
                var prev = _isMoving;
                _isMoving = value;

                if (prev && !value)
                {
                    StopMoving?.Invoke();
                }
                else if (!prev && value)
                {
                    StartMoving?.Invoke();
                }
            }
        }

        public bool IsFalling { get; protected set; }

        private int _groundContacts;
        public bool IsGrounded => _groundContacts > 0;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start()
        {
            Landed += OnLanded;

            NavMeshAgent.updatePosition = false;
        }

        protected virtual void Update()
        {
            HandleNavigationMovement();
        }

        protected virtual void FixedUpdate()
        {
            var now = Time.time;
            if (IsGrounded)
            {
                _timeLastGrounded = now;
            }

            if (!IsFalling && now - _timeLastGrounded > DelayBeforeInferringFall)
            {
                IsFalling = true;
                _animationParams.FallTriggered = true;
            }

            UpdateAnimationState();
        }

        private void HandleNavigationMovement()
        {
            if (NavMeshAgent.enabled)
            {
                if (!IsFalling && IsGrounded)
                {
                    var targetPos = NavMeshAgent.nextPosition;
                    var targetDirection = targetPos - Rigidbody.position;
                    targetDirection.y = 0;
                    targetDirection = targetDirection.normalized;

                    var desiredVelocity = targetDirection * (IsSprinting ? sprintSpeed : normalSpeed);
                    Rigidbody.linearVelocity =
                        new Vector3(desiredVelocity.x, Rigidbody.linearVelocity.y, desiredVelocity.z);

                    if (desiredVelocity != Vector3.zero)
                    {
                        IsMoving = true;

                        // Rotate the entity to face movement direction
                        var qA = Rigidbody.rotation;
                        var qB = Quaternion.LookRotation(desiredVelocity, UpDirection);
                        Rigidbody.MoveRotation(Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.deltaTime)));
                    }
                    else
                    {
                        IsMoving = false;
                    }
                }

                // Sync the agent's internal position back to the object to prevent drifting
                NavMeshAgent.nextPosition = transform.position;
            }
        }

        public Vector3 UpDirection => UpMode == UpModes.World ? Vector3.up : transform.up;

        private void OnLanded()
        {
            _animationParams.LandTriggered = true;
        }

        public void Jump()
        {
            if (!IsFalling && IsGrounded)
            {
                _animationParams.JumpTriggered = true;
                IsFalling = true;
                Rigidbody.AddForce(UpDirection * (IsSprinting ? sprintJumpForce : normalJumpForce),
                    ForceMode.Impulse);
            }
        }

        private void UpdateAnimationState()
        {
            var linearVelocityXZ = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
            var forwardVelocity = Quaternion.Inverse(Rigidbody.rotation) * linearVelocityXZ;

            var speed = forwardVelocity.magnitude;

            // Hysteresis reduction
            _animationParams.IsRunning = speed > normalSpeed * 2 + (_animationParams.IsRunning ? -0.15f : 0.15f);

            // Set the normalized direction of motion and scale the animation speed to match motion speed
            var targetDirection = speed > IdleThreshold ? forwardVelocity / normalSpeed : Vector3.zero;
            _animationParams.Direction = Vector3.Slerp(_animationParams.Direction, targetDirection,
                Damper.Damp(1, damping, Time.deltaTime));
            _animationParams.MotionScale = IsMoving ? speed / (IsSprinting ? sprintSpeed : normalSpeed) : 1;
            _animationParams.JumpScale = jumpBaseScale * (IsSprinting ? normalJumpForce / sprintJumpForce : 1);

            // We scale the sprint animation speed to loosely match the actual speed, but we cheat
            // at the high end to avoid making the animation look ridiculous
            if (_animationParams.IsRunning)
            {
                _animationParams.MotionScale = (speed < sprintSpeed)
                    ? speed / sprintSpeed
                    : Mathf.Min(maxSprintScale, 1 + (speed - sprintSpeed) / (3 * sprintSpeed));
            }

            UpdateAnimation();

            _animationParams.JumpTriggered = false;
            _animationParams.FallTriggered = false;
            _animationParams.LandTriggered = false;
        }

        private void UpdateAnimation()
        {
            Animator.SetBool(AnimationParams.MovingKey, IsMoving);
            Animator.SetFloat(AnimationParams.ForwardKey, _animationParams.Direction.z);
            Animator.SetFloat(AnimationParams.StrafeKey, _animationParams.Direction.x);
            Animator.SetFloat(AnimationParams.MotionScaleKey, _animationParams.MotionScale);
            Animator.SetFloat(AnimationParams.JumpScaleKey, _animationParams.JumpScale);

            if (_animationParams.JumpTriggered)
            {
                Animator.SetTrigger(AnimationParams.JumpKey);
            }

            if (_animationParams.FallTriggered)
            {
                Animator.SetTrigger(AnimationParams.FallKey);
            }

            if (_animationParams.LandTriggered)
            {
                Animator.SetTrigger(AnimationParams.LandKey);
            }
        }

        public void StartMoveTo(Vector3 destination)
        {
            NavMeshAgent.destination = destination;
        }

        protected void OnCollisionEnter(Collision other)
        {
            // TODO: Check collider tag or mask
            _groundContacts++;

            if (IsGrounded && IsFalling)
            {
                IsFalling = false;
                Landed?.Invoke();
            }
        }

        protected void OnCollisionExit(Collision other)
        {
            _groundContacts--;

            if (_groundContacts < 0)
            {
                _groundContacts = 0;
            }
        }
    }
}