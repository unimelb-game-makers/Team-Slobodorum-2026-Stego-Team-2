using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class ClimbStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var movement = animator.GetComponent<HumanoidMovement>();
            movement.IsClimbing = false;
            animator.applyRootMotion = false;
        }
    }
}