using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAssassinated : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Assassinated", false);
        animator.SetFloat("isForward", 0);
    }
}
