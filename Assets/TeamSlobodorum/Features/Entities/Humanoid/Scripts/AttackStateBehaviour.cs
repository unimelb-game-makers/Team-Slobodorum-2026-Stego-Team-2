using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var movement = animator.GetComponent<HumanoidMovement>();
            movement.IsAttacking = false;
        }
    }
}
