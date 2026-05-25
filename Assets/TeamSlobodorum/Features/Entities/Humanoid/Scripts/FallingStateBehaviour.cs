using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class FallingStateBehaviour : StateMachineBehaviour
    {
        private static readonly int FallProgressKey = Animator.StringToHash("FallProgress");
        
        private HumanoidMovement _movement;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<HumanoidMovement>();
            }
            _movement.IsFalling = true;
            animator.SetFloat(FallProgressKey, 0);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var progress = animator.GetFloat(FallProgressKey);
            animator.SetFloat(FallProgressKey, Mathf.MoveTowards(progress, 1, Time.deltaTime));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_movement)
            {
                _movement = animator.GetComponent<HumanoidMovement>();
            }
            _movement.IsFalling = false;
        }
    }
}