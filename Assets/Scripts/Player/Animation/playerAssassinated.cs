using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAssassinated : StateMachineBehaviour
{
    private int hashCanMove = Animator.StringToHash("isMoveAble");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashCanMove, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f)
        {
            animator.SetBool("Assassinated", false);
            animator.SetBool(hashCanMove, true);
            animator.SetFloat("isForward", 0);
        }   
    }
}
