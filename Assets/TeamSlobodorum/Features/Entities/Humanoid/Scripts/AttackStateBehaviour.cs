using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        private Movement _movement;
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<Movement>();
            }
            _movement.IsAttacking = false;
        }
    }
}
