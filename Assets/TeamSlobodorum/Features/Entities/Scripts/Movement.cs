using System;
using UnityEngine;
using UnityEngine.AI;

namespace TeamSlobodorum.Entities
{
    [RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent),  typeof(Animator))]
    public abstract class Movement : MonoBehaviour
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
        
        [Tooltip("Never speed up the sprint animation more than this, to avoid absurdly fast movement")]
        public float maxSprintScale = 3f;

        [Tooltip("Scale factor for the overall speed of the jump animation")]
        public float jumpBaseScale = 1f;

        [Header("Step Up")]
        public float stepAmount = 2f;
        public Transform stepRayUpper;
        public Transform stepRayLower;
        
        [Header("Navigation")]
        public float linkTraversalSpeed = 2.5f;
        
        public event Action StartMoving;
        public event Action StopMoving;
        public event Action PreventMovementChanged;
        public event Action FallTriggered;

        public Rigidbody Rigidbody { get; private set; }
        protected NavMeshAgent NavMeshAgent { get; private set; }
        protected Animator Animator { get; private set; }

        protected float DefaultStoppingDistance { get; private set; }
        
        private const float DelayBeforeInferringFall = 0.3f;
        private const float DelayBeforeInferringMove = 0.3f;
        private const float FallingTime = 1.5f;
        private float _timeLastGrounded;
        private float _timeLastMoved;
        
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
        
        public bool IsFalling { get; set; }
        public bool IsAttacking { get; set; }

        public virtual bool CanMove => !PreventMovement && !IsFalling;
        public virtual bool CanPerformAction => !PreventMovement && !IsFalling && IsGrounded;

        public bool LastMoveSucceeded { get; protected set; }
        public bool IsSprinting { get; set; }

        private bool _preventMovement;

        public bool PreventMovement
        {
            get => _preventMovement;
            set
            {
                var prev = _preventMovement;
                _preventMovement = value;
                if (prev != value)
                {
                    PreventMovementChanged?.Invoke();
                }
            }
        }
        
        public bool IsGrounded { get; private set; }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            DefaultStoppingDistance = NavMeshAgent.stoppingDistance;
        }

        protected virtual void Start()
        {
            NavMeshAgent.updatePosition = false;
            NavMeshAgent.updateRotation = false;
            PreventMovementChanged += OnPreventMovementChanged;
        }

        protected virtual void FixedUpdate()
        {
            var now = Time.fixedTime;
            if (IsGrounded && now - _timeLastGrounded > DelayBeforeInferringFall)
            {
                _timeLastGrounded = now;
            }
            else if (!IsFalling && now - _timeLastGrounded > FallingTime)
            {
                IsFalling = true;
                FallTriggered?.Invoke();
            }
            
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

        public virtual void StartMovingTo(Vector3 destination)
        {
            StartMovingTo(destination, DefaultStoppingDistance);
        }

        public virtual void StartMovingTo(Vector3 destination, float stoppingDistance)
        {
            NavMeshAgent.destination = destination;
            NavMeshAgent.stoppingDistance = stoppingDistance;
            IsMoving = true;
        }
        
        protected virtual void OnCollisionStay(Collision collision)
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

        protected virtual void OnCollisionExit(Collision collision)
        {
            IsGrounded = false;
        }
        
        public void SyncNavMeshAgentToTransform()
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 1f, NavMesh.AllAreas))
            {
                NavMeshAgent.Warp(hit.position);
            }
        }
        
        private void OnPreventMovementChanged()
        {
            if (!PreventMovement)
            {
                SyncNavMeshAgentToTransform();
            }
        }
        
        protected bool TryStepUp(Vector3 direction)
        {
            if (Physics.Raycast(stepRayLower.transform.position, direction,
                    out _, 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (!Physics.Raycast(stepRayUpper.transform.position, direction,
                        out _, 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    Rigidbody.position -= new Vector3(0f, -stepAmount * Time.deltaTime, 0f);
                    return true;
                }
            }

            return false;
        }
        
        protected void NotifyMovement()
        {
            _timeLastMoved = Time.fixedTime;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            DrawStepUpRays(transform.forward);
            DrawStepUpRays(transform.TransformDirection(0.5f, 0, 1));
            DrawStepUpRays(transform.TransformDirection(-0.5f, 0, 1));
        }

        private void DrawStepUpRays(Vector3 direction)
        {
            if (stepRayLower && stepRayUpper)
            {
                Gizmos.DrawRay(stepRayLower.transform.position, direction * 0.5f);
                Gizmos.DrawRay(stepRayUpper.transform.position, direction * 0.5f);
            }
        }
    }
}