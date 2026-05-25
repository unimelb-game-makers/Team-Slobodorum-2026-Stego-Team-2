using Unity.Cinemachine;
using UnityEngine;

namespace TeamSlobodorum.Entities.Enemies
{
    public class ChessPieceMovement : Movement
    {
        protected struct ChessPieceAnimationParams
        {
            public static readonly int AttackKey = Animator.StringToHash("Attack");

            public bool AttackTriggered;
        }
        private ChessPieceAnimationParams _animationParams;
        
        private Vector3 _navmeshLinkStartPos;
        private Vector3 _navmeshLinkEndPos;
        private float _navmeshLinkProgress;
        
        protected override void FixedUpdate()
        {
            HandleNavigationMovement();
            UpdateAnimation();

            if (IsFalling && IsGrounded)
            {
                IsFalling = false;
            }

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

                        if (IsGrounded)
                        {
                            Jump();
                        }

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
                Rigidbody.AddForce(transform.up * (IsSprinting ? sprintJumpForce : normalJumpForce),
                    ForceMode.Impulse);
            }
        }
        
        public void StartMeleeAttack()
        {
            if (CanPerformAction)
            {
                IsAttacking = true;
                _animationParams.AttackTriggered = true;
                Rigidbody.linearVelocity = new Vector3(0, Rigidbody.linearVelocity.y, 0);
            }
        }
        
        protected virtual void UpdateAnimation()
        {
            if (_animationParams.AttackTriggered)
            {
                Animator.SetTrigger(ChessPieceAnimationParams.AttackKey);
                _animationParams.AttackTriggered = false;
            }
        }
    }
}