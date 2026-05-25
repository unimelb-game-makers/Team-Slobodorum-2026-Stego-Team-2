using TeamSlobodorum.Entities.Player;
using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class ClimbStateBehaviour : StateMachineBehaviour
    {
        private PlayerMovement _movement;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<PlayerMovement>();
            }
            _movement.IsClimbing = false;
            animator.applyRootMotion = false;
        }
    }
}