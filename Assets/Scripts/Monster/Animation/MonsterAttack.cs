using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
        animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        animator.SetLayerWeight(1, 1);
    }
}