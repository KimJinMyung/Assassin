using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryAnimation : StateMachineBehaviour
{
    private PlayerBattleManager owner;

    [SerializeField] float ParringTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", false);
        animator.SetBool("ParryAble", false);
        owner = animator.GetComponent<PlayerBattleManager>();
        owner.isParried = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime <= ParringTime)
        {
            animator.SetBool("Parring", true);
        }
        else
        {
            animator.SetBool("Parring", false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isMoveAble", true);
        animator.SetBool("ParryAble", true);
        owner.isParried = false;
    }
}
