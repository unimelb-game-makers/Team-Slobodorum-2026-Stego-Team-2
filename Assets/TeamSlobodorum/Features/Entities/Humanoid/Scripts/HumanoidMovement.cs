using Unity.Cinemachine;
using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    [RequireComponent(typeof(Rigidbody))]
    public class HumanoidMovement : Movement
    {
        protected struct HumanoidAnimationParams
        {
            public static readonly int MovingKey = Animator.StringToHash("Moving");
            public static readonly int ForwardKey = Animator.StringToHash("Forward");
            public static readonly int StrafeKey = Animator.StringToHash("Strafe");
            public static readonly int MotionScaleKey = Animator.StringToHash("MotionScale");
            public static readonly int JumpScaleKey = Animator.StringToHash("JumpScale");
            public static readonly int FallKey = Animator.StringToHash("Fall");
            public static readonly int MeleeKey = Animator.StringToHash("Melee");
            public static readonly int JumpKey = Animator.StringToHash("Jump");
            public static readonly int GroundedKey = Animator.StringToHash("Grounded");

            public bool IsRunning;
            public bool FallTriggered;
            public bool MeleeTriggered;
            public bool JumpTriggered;
            public Vector3 Direction; // normalized direction of motion
            public float MotionScale; // scale factor for the animation speed
            public float JumpScale; // scale factor for the jump animation
        }

        protected Humanoid Humanoid { get; private set; }

        private HumanoidAnimationParams _animationParams;
        private const float IdleThreshold = 0.2f;

        private Vector3 _navmeshLinkStartPos;
        private Vector3 _navmeshLinkEndPos;
        private float _navmeshLinkProgress;
        
        public bool IsJumping { get; set; }

        public override bool CanMove => base.CanMove && !IsAttacking && !IsJumping;
        public override bool CanPerformAction => base.CanPerformAction && !IsAttacking && !IsJumping;

        protected override void Awake()
        {
            base.Awake();
            Humanoid = GetComponent<Humanoid>();
            
            FallTriggered += () => _animationParams.FallTriggered = true;
        }

        protected override void FixedUpdate()
        {
            HandleNavigationMovement();
            UpdateAnimationState();

            base.FixedUpdate();
        }

        private void HandleNavigationMovement()
        {
            if (NavMeshAgent.enabled && NavMeshAgent.isOnOffMeshLink ||
                (IsMoving && NavMeshAgent.isOnNavMesh))
            {
                var pathFinished = !NavMeshAgent.pathPending &&
                                   NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance;
                if (NavMeshAgent.isStopped || pathFinished)
                {
                    LastMoveSucceeded = pathFinished;
                    IsMoving = false;
                    return;
                }

                if (NavMeshAgent.isOnOffMeshLink || Rigidbody.isKinematic)
                {
                    if (_navmeshLinkProgress == 0)
                    {
                        _navmeshLinkStartPos = Rigidbody.position;
                        _navmeshLinkEndPos = NavMeshAgent.currentOffMeshLinkData.endPos;
                        Rigidbody.isKinematic = true;

                        IsJumping = true;
                        _animationParams.JumpTriggered = true;
                    }

                    if (_navmeshLinkProgress < 1f)
                    {
                        var distance = Vector3.Distance(_navmeshLinkStartPos, _navmeshLinkEndPos);
                        _navmeshLinkProgress += Time.fixedDeltaTime * (linkTraversalSpeed / distance);
                        var currentPos = Vector3.Lerp(_navmeshLinkStartPos, _navmeshLinkEndPos,
                            _navmeshLinkProgress);
                        var height = distance * 0.5f;
                        var yOffset = Mathf.Sin(_navmeshLinkProgress * Mathf.PI) * height;
                        currentPos.y += yOffset;
                        transform.position = currentPos;
                    }
                    else
                    {
                        Rigidbody.isKinematic = false;
                        NavMeshAgent.CompleteOffMeshLink();
                        _navmeshLinkProgress = 0;
                    }
                }
                else if (CanMove)
                {
                    var moveDirection = NavMeshAgent.desiredVelocity;
                    moveDirection.y = 0;

                    if (moveDirection.sqrMagnitude > 0.01f)
                    {
                        NotifyMovement();
                        moveDirection.Normalize();
                        var moveVelocity = moveDirection * (IsSprinting ? sprintSpeed : normalSpeed);
                        Rigidbody.linearVelocity =
                            new Vector3(moveVelocity.x, Rigidbody.linearVelocity.y, moveVelocity.z);

                        // Rotate the entity to face movement direction
                        var qA = Rigidbody.rotation;
                        var qB = Quaternion.LookRotation(moveVelocity, Vector3.up);
                        Rigidbody.MoveRotation(Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.fixedDeltaTime)));
                    }
                }

                if (Vector3.Distance(transform.position, NavMeshAgent.nextPosition) > 2f)
                {
                    SyncNavMeshAgentToTransform();
                }

                NavMeshAgent.nextPosition = Rigidbody.position;
            }
        }

        public void Jump()
        {
            if (CanPerformAction)
            {
                IsJumping = true;
                _animationParams.JumpTriggered = true;
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

        protected virtual void UpdateAnimation()
        {
            Animator.SetBool(HumanoidAnimationParams.MovingKey, IsMoving);
            Animator.SetFloat(HumanoidAnimationParams.ForwardKey, _animationParams.Direction.z);
            Animator.SetFloat(HumanoidAnimationParams.StrafeKey, _animationParams.Direction.x);
            Animator.SetFloat(HumanoidAnimationParams.MotionScaleKey, _animationParams.MotionScale);
            Animator.SetFloat(HumanoidAnimationParams.JumpScaleKey, _animationParams.JumpScale);
            Animator.SetBool(HumanoidAnimationParams.GroundedKey, IsGrounded);

            if (_animationParams.FallTriggered)
            {
                Animator.SetTrigger(HumanoidAnimationParams.FallKey);
                _animationParams.FallTriggered = false;
            }

            if (_animationParams.MeleeTriggered)
            {
                Animator.SetTrigger(HumanoidAnimationParams.MeleeKey);
                _animationParams.MeleeTriggered = false;
            }

            if (_animationParams.JumpTriggered)
            {
                Animator.SetTrigger(HumanoidAnimationParams.JumpKey);
                _animationParams.JumpTriggered = false;
            }
        }
    }
}