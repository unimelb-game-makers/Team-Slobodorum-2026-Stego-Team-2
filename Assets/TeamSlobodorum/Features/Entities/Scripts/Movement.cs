using System;
using UnityEngine;

namespace TeamSlobodorum.Entities
{
    public abstract class Movement : MonoBehaviour
    {
        public event Action StartMoving;
        public event Action StopMoving;
        
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
        public bool LastMoveSucceeded { get; protected set; }
        public bool IsSprinting { get; set; }
        public bool PreventMovement { get; set; }
        
        public abstract void StartMoveTo(Vector3 destination);
    }
}
