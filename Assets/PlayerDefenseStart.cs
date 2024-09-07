using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefenseStart : StateMachineBehaviour
{
    private readonly int hashPull = Animator.StringToHash("Pull");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(hashPull);
        animator.ResetTrigger(hashAttack);
        animator.SetInteger(hashAttackIndex, 0);
    }
}
