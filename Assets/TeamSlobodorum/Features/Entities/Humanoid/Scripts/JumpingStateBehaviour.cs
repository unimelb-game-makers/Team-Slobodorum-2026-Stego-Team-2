using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class JumpingStateBehaviour : StateMachineBehaviour
    {
        private HumanoidMovement _movement;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<HumanoidMovement>();
            }
            _movement.IsJumping = true;
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<HumanoidMovement>();
            }
            _movement.IsJumping = false;
        }
    }
}