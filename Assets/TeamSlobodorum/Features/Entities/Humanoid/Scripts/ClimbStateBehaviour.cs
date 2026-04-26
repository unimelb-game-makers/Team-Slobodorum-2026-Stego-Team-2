using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class ClimbStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var movement = animator.GetComponent<HumanoidMovement>();
            movement.IsClimbing = false;
        }
    }
}