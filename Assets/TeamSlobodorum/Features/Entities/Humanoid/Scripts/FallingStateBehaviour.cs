using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class FallingStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var movement = animator.GetComponent<HumanoidMovement>();
            movement.IsFalling = false;
            movement.IsJumping = false;
        }
    }
}