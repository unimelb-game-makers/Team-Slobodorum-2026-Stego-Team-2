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
        public Animator animator;

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
            public static readonly int MeleeKey = Animator.StringToHash("Melee");
            public static readonly int JumpKey = Animator.StringToHash("Jump");
            public static readonly int ClimbKey = Animator.StringToHash("Climb");
            public static readonly int GroundedKey = Animator.StringToHash("Grounded");

            public bool IsRunning;
            public bool FallTriggered;
            public bool MeleeTriggered;
            public bool JumpTriggered;
            public bool ClimbTriggered;
            public Vector3 Direction; // normalized direction of motion
            public float MotionScale; // scale factor for the animation speed
            public float JumpScale; // scale factor for the jump animation
        }

        public Rigidbody Rigidbody { get; private set; }
        protected NavMeshAgent NavMeshAgent { get; private set; }
        protected Humanoid Humanoid { get; private set; }

        private AnimationParams _animationParams;
        private const float IdleThreshold = 0.2f;

        private const float DelayBeforeInferringFall = 0.3f;
        private const float DelayBeforeInferringMove = 0.3f;
        private const float FallingTime = 1.5f;
        private const float AirControlTime = 0.5f;
        private float _timeLastGrounded;
        private float _timeLastMoved;
        private Vector3 _ledgePosition;

        public bool IsFalling { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsClimbing { get; set; }
        public bool IsForwardObstructed { get; private set; }

        public override bool CanMove =>
            base.CanMove && !IsAttacking && !IsClimbing && !IsFalling &&
            (!IsJumping || Time.fixedTime - _timeLastGrounded <= AirControlTime);

        public override bool CanPerformAction =>
            base.CanMove && !IsAttacking && !IsClimbing && !IsFalling && !IsJumping && IsGrounded;

        public bool IsGrounded { get; private set; }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected virtual void OnValidate()
        {
            Humanoid = GetComponent<Humanoid>();
        }

        protected virtual void Start()
        {
            NavMeshAgent.updatePosition = false;
            NavMeshAgent.updateRotation = false;
            PreventMovementChanged += OnPreventMovementChanged;
        }

        protected virtual void Update()
        {
            if (IsClimbing)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                var progress = stateInfo.normalizedTime;
                if (progress < 0.35f)
                {
                    var yDiff = Humanoid.rightHand.position.y - _ledgePosition.y;
                    var targetPosition = transform.position - new Vector3(0, yDiff, 0);
                    transform.position = Vector3.Slerp(transform.position, targetPosition,
                        Damper.Damp(1, damping, Time.fixedDeltaTime));
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            var now = Time.fixedTime;
            if (IsGrounded && now - _timeLastGrounded > DelayBeforeInferringFall)
            {
                _timeLastGrounded = now;

                if (IsFalling || IsJumping)
                {
                    Landed?.Invoke();
                }
            }
            else if (!IsFalling && now - _timeLastGrounded > FallingTime)
            {
                IsFalling = true;
                _animationParams.FallTriggered = true;
            }

            IsForwardObstructed = Physics.Raycast(Humanoid.stepRayUpper.transform.position, transform.forward,
                out _, 0.5f);

            HandleNavigationMovement();
            UpdateAnimationState();

            if (!CanMove && now - _timeLastMoved > DelayBeforeInferringMove)
            {
                IsMoving = false;
            }

            if (IsMoving)
            {
                _ = TryStepUp(transform.forward) ||
                    TryStepUp(transform.TransformDirection(0.5f, 0, 1)) ||
                    TryStepUp(transform.TransformDirection(-0.5f, 0, 1));
            }
        }

        private void HandleNavigationMovement()
        {
            if (NavMeshAgent.enabled && IsMoving && NavMeshAgent.isOnNavMesh)
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
                    var moveDirection = NavMeshAgent.desiredVelocity;
                    moveDirection.y = 0;

                    if (moveDirection.sqrMagnitude > 0.01f)
                    {
                        NotifyMovement();
                        moveDirection.Normalize();
                        var moveVelocity = moveDirection * (IsSprinting ? sprintSpeed : normalSpeed);
                        Rigidbody.linearVelocity = moveVelocity;

                        // Rotate the entity to face movement direction
                        var qA = Rigidbody.rotation;
                        var qB = Quaternion.LookRotation(moveVelocity, Vector3.up);
                        Rigidbody.MoveRotation(Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.fixedDeltaTime)));
                    }
                }

                NavMeshAgent.nextPosition = Rigidbody.position;
            }
        }

        private void OnPreventMovementChanged()
        {
            if (!PreventMovement)
            {
                NavMeshAgent.Warp(Rigidbody.position);
            }
        }

        public void Jump()
        {
            if (!IsClimbing)
            {
                if ((Physics.Raycast(Humanoid.climbRayLower.transform.position, transform.forward,
                         out _, 0.5f) ||
                     Physics.Raycast(Humanoid.stepRayUpper.transform.position, transform.forward,
                         out _, 0.5f)) &&
                    !Physics.Raycast(Humanoid.climbRayUpper.transform.position, transform.forward,
                        out _, 0.5f))
                {
                    var originDown = Humanoid.climbRayUpper.transform.position + transform.forward * 0.5f;
                    if (Physics.Raycast(originDown, Vector3.down, out var ledgeHit, 1.3f))
                    {
                        _ledgePosition = ledgeHit.point;
                        IsClimbing = true;
                        _animationParams.ClimbTriggered = true;
                        return;
                    }
                }
            }

            if (CanPerformAction)
            {
                IsJumping = true;
                _animationParams.JumpTriggered = true;
                Rigidbody.AddForce(transform.up * (IsSprinting ? sprintJumpForce : normalJumpForce),
                    ForceMode.Impulse);
            }
        }

        protected void NotifyMovement()
        {
            _timeLastMoved = Time.fixedTime;
        }

        public void StartMeleeAttack()
        {
            if (CanPerformAction)
            {
                IsAttacking = true;
                _animationParams.MeleeTriggered = true;
                Rigidbody.linearVelocity = new Vector3(0, Rigidbody.linearVelocity.y, 0);
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
            animator.SetBool(AnimationParams.MovingKey, IsMoving);
            animator.SetFloat(AnimationParams.ForwardKey, _animationParams.Direction.z);
            animator.SetFloat(AnimationParams.StrafeKey, _animationParams.Direction.x);
            animator.SetFloat(AnimationParams.MotionScaleKey, _animationParams.MotionScale);
            animator.SetFloat(AnimationParams.JumpScaleKey, _animationParams.JumpScale);
            animator.SetBool(AnimationParams.GroundedKey, IsGrounded);

            if (_animationParams.FallTriggered)
            {
                animator.SetTrigger(AnimationParams.FallKey);
                _animationParams.FallTriggered = false;
            }

            if (_animationParams.MeleeTriggered)
            {
                animator.SetTrigger(AnimationParams.MeleeKey);
                _animationParams.MeleeTriggered = false;
            }

            if (_animationParams.JumpTriggered)
            {
                animator.SetTrigger(AnimationParams.JumpKey);
                _animationParams.JumpTriggered = false;
            }

            if (_animationParams.ClimbTriggered)
            {
                animator.SetTrigger(AnimationParams.ClimbKey);
                _animationParams.ClimbTriggered = false;
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
                    out _, 0.5f))
            {
                if (!Physics.Raycast(Humanoid.stepRayUpper.transform.position, direction,
                        out _, 0.5f))
                {
                    Rigidbody.position -= new Vector3(0f, -stepAmount * Time.deltaTime, 0f);
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Humanoid.climbRayUpper.transform.position, transform.forward * 0.5f);
            Gizmos.DrawRay(Humanoid.climbRayLower.transform.position, transform.forward * 0.5f);
            DrawStepUpRays(transform.forward);
            DrawStepUpRays(transform.TransformDirection(0.5f, 0, 1));
            DrawStepUpRays(transform.TransformDirection(-0.5f, 0, 1));
        }

        private void DrawStepUpRays(Vector3 direction)
        {
            Gizmos.DrawRay(Humanoid.stepRayUpper.transform.position, direction * 0.5f);
            Gizmos.DrawRay(Humanoid.stepRayUpper.transform.position, direction * 0.5f);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                // Cos(70 degree) = 0.34
                if (contact.normal.y > 0.34f)
                {
                    Debug.DrawLine(contact.point, contact.point + contact.normal, Color.purple);
                    IsGrounded = true;
                    break;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            IsGrounded = false;
        }
    }
}