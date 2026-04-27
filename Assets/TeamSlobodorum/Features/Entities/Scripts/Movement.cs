using System;
using UnityEngine;

namespace TeamSlobodorum.Entities
{
    public abstract class Movement : MonoBehaviour
    {
        public event Action StartMoving;
        public event Action StopMoving;
        public event Action PreventMovementChanged;

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

        public virtual bool CanMove => !PreventMovement;
        public virtual bool CanPerformAction => CanMove;
        
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

        public abstract void StartMovingTo(Vector3 destination, float stoppingDistance = 0);
    }
}