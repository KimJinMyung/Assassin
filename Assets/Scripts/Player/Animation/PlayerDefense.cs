using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefense : StateMachineBehaviour
{
    private int hashParrAble = Animator.StringToHash("ParryAble");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashParrAble, false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashParrAble, true);
    }
}
