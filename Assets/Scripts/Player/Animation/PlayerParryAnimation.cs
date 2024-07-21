using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryAnimation : StateMachineBehaviour
{
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ParryAble", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime <= 0.5f)
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
        animator.SetBool("ParryAble", true);
    }
}
