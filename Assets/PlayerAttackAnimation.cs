using UnityEngine;

public class PlayerAttackAnimation : StateMachineBehaviour
{
    private int _attackCount = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackCount = animator.GetInteger("AttackIndex");
        _attackCount = (_attackCount + 1) % 4;
        animator.applyRootMotion = true;
        animator.SetInteger("AttackIndex", _attackCount);
        animator.SetBool("AttackAble", false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        _attackCount = 0;
        animator.SetInteger("AttackIndex", 0);
        animator.ResetTrigger("Attack");
        animator.SetBool("AttackAble", true);
        animator.SetBool("isMoveAble", true);
        animator.applyRootMotion =false;
    }
}
