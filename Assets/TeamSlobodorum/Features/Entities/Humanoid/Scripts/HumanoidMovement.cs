using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

namespace TeamSlobodorum.Entities.Humanoid
{
    [RequireComponent(typeof(Rigidbody))]
    public class HumanoidMovement : Movement
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

        [Header("IK and Step Up")]
        [SerializeField] private float distanceToGround = 0.08f;

        [SerializeField] private float stepAmount = 2f;
        [SerializeField] private float ikSmooth = 15f;

        [Header("Animation")]
        [Tooltip("Never speed up the sprint animation more than this, to avoid absurdly fast movement")]
        public float maxSprintScale = 3f;

        [Tooltip("Scale factor for the overall speed of the jump animation")]
        public float jumpBaseScale = 1f;

        public event Action Landed;

        protected struct AnimationParams
        {
            public static readonly int MovingKey = Animator.StringToHash("Moving");
            public static readonly int ForwardKey = Animator.StringToHash("Forward");
            public static readonly int StrafeKey = Animator.StringToHash("Strafe");
            public static readonly int MotionScaleKey = Animator.StringToHash("MotionScale");
            public static readonly int JumpScaleKey = Animator.StringToHash("JumpScale");
            public static readonly int FallKey = Animator.StringToHash("Fall");
            public static readonly int LandKey = Animator.StringToHash("Land");
            public static readonly int MeleeKey = Animator.StringToHash("Melee");

            public bool IsRunning;
            public bool FallTriggered;
            public bool LandTriggered;
            public bool MeleeTriggered;
            public Vector3 Direction; // normalized direction of motion
            public float MotionScale; // scale factor for the animation speed
            public float JumpScale; // scale factor for the jump animation
        }

        public Rigidbody Rigidbody { get; private set; }
        protected Animator Animator { get; private set; }
        protected NavMeshAgent NavMeshAgent { get; private set; }
        protected Humanoid Humanoid { get; private set; }

        private AnimationParams _animationParams;
        private const float IdleThreshold = 0.2f;

        private const float DelayBeforeInferringFall = 0.3f;
        private const float FallingTime = 1f;
        private const float AirControlTime = 1f;
        private float _timeLastGrounded;
        private float _timeLastJump;

        public bool IsFalling { get; protected set; }
        public bool IsAttacking { get; set; }

        public override bool CanMove =>
            base.CanMove && !IsAttacking && (!IsFalling || Time.time - _timeLastJump <= AirControlTime);

        public override bool CanPerformAction => base.CanMove && !IsAttacking && !IsFalling && IsGrounded();

        public bool IsGrounded()
        {
            return Physics.SphereCast(Humanoid.stepRayUpper.transform.position, 0.25f, -Vector3.up, out _,
                0.6f);
        }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected virtual void OnValidate()
        {
            Humanoid = GetComponent<Humanoid>();
        }

        protected virtual void Start()
        {
            Landed += OnLanded;

            NavMeshAgent.updatePosition = false;
            NavMeshAgent.updateRotation = false;
        }

        protected virtual void FixedUpdate()
        {
            var now = Time.time;
            if (IsGrounded() && now - _timeLastGrounded > DelayBeforeInferringFall)
            {
                _timeLastGrounded = now;

                if (IsFalling)
                {
                    IsFalling = false;
                    Landed?.Invoke();
                }
            }

            if (!IsFalling && now - _timeLastGrounded > FallingTime)
            {
                IsFalling = true;
                _animationParams.FallTriggered = true;
            }

            if (_animationParams.MeleeTriggered)
            {
                Rigidbody.linearVelocity = new Vector3(0, Rigidbody.linearVelocity.y, 0);
            }

            HandleNavigationMovement();
            UpdateAnimationState();

            if (IsMoving)
            {
                _ = TryStepUp(transform.forward) ||
                    TryStepUp(transform.TransformDirection(0.5f, 0, 1)) ||
                    TryStepUp(transform.TransformDirection(-0.5f, 0, 1));
            }
        }

        private void HandleNavigationMovement()
        {
            if (NavMeshAgent.enabled && IsMoving)
            {
                var pathFinished = !NavMeshAgent.pathPending &&
                                   NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance;
                if (NavMeshAgent.isStopped || pathFinished)
                {
                    LastMoveSucceeded = pathFinished;
                    IsMoving = false;
                    return;
                }

                if (CanMove)
                {
                    var desiredVelocity = NavMeshAgent.desiredVelocity;
                    desiredVelocity.y = 0;

                    if (desiredVelocity.sqrMagnitude > 0.01f)
                    {
                        var moveVelocity = desiredVelocity.normalized * (IsSprinting ? sprintSpeed : normalSpeed);
                        Rigidbody.linearVelocity = moveVelocity;

                        // Rotate the entity to face movement direction
                        var qA = Rigidbody.rotation;
                        var qB = Quaternion.LookRotation(moveVelocity, Vector3.up);
                        Rigidbody.MoveRotation(Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.fixedDeltaTime)));
                    }
                }

                // Sync the agent's internal position back to the object to prevent drifting
                NavMeshAgent.nextPosition = Rigidbody.position;
            }
        }

        private void OnLanded()
        {
            _animationParams.LandTriggered = true;
        }

        public void Jump()
        {
            if (CanPerformAction)
            {
                _timeLastJump = Time.time;
                _animationParams.FallTriggered = true;
                IsFalling = true;
                Rigidbody.AddForce(transform.up * (IsSprinting ? sprintJumpForce : normalJumpForce),
                    ForceMode.Impulse);
            }
        }

        public void StartMeleeAttack()
        {
            if (CanPerformAction)
            {
                IsAttacking = true;
                _animationParams.MeleeTriggered = true;
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
                Damper.Damp(1, damping, Time.fixedDeltaTime));
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
        }

        private void UpdateAnimation()
        {
            Animator.SetBool(AnimationParams.MovingKey, IsMoving);
            Animator.SetFloat(AnimationParams.ForwardKey, _animationParams.Direction.z);
            Animator.SetFloat(AnimationParams.StrafeKey, _animationParams.Direction.x);
            Animator.SetFloat(AnimationParams.MotionScaleKey, _animationParams.MotionScale);
            Animator.SetFloat(AnimationParams.JumpScaleKey, _animationParams.JumpScale);

            if (_animationParams.FallTriggered)
            {
                Animator.SetTrigger(AnimationParams.FallKey);
                _animationParams.FallTriggered = false;
            }

            if (_animationParams.LandTriggered)
            {
                Animator.SetTrigger(AnimationParams.LandKey);
                _animationParams.LandTriggered = false;
            }

            if (_animationParams.MeleeTriggered)
            {
                Animator.SetTrigger(AnimationParams.MeleeKey);
                _animationParams.MeleeTriggered = false;
            }
        }

        public override void StartMovingTo(Vector3 destination, float stoppingDistance = 0)
        {
            NavMeshAgent.destination = destination;
            NavMeshAgent.stoppingDistance = stoppingDistance;
            IsMoving = true;
        }

        private bool TryStepUp(Vector3 direction)
        {
            if (Physics.Raycast(Humanoid.stepRayLower.transform.position, direction,
                    out _, 0.4f))
            {
                if (!Physics.Raycast(Humanoid.stepRayUpper.transform.position, direction,
                        out _, 0.4f))
                {
                    Rigidbody.position -= new Vector3(0f, -stepAmount * Time.deltaTime, 0f);
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            DrawStepUpRays(transform.forward);
            DrawStepUpRays(transform.TransformDirection(0.5f, 0, 1));
            DrawStepUpRays(transform.TransformDirection(-0.5f, 0, 1));
        }

        private void DrawStepUpRays(Vector3 direction)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Humanoid.stepRayLower.transform.position, direction * 0.4f);
            Gizmos.DrawRay(Humanoid.stepRayUpper.transform.position, direction * 0.4f);
        }
    }
}