using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack_NextAttackAble : StateMachineBehaviour
{
    [SerializeField] private float MoveStartTime;
    [SerializeField] private float MoveEndTime;

    [SerializeField] private float NextAttackAbleTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", stateInfo.normalizedTime < 0.1f);

        if (stateInfo.normalizedTime >= NextAttackAbleTime)
        {
            animator.SetBool("AttackAble", true);
        }
        else
        {
            animator.SetBool("AttackAble", false);
        }

    }
}
