using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class MonsterAttack : StateMachineBehaviour
{
    BehaviorTree behaviourTree;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        behaviourTree = animator.GetComponent<BehaviorTree>();

        animator.applyRootMotion = true;
        animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        if(!(bool)behaviourTree.GetVariable("isDead").GetValue() && !(bool)behaviourTree.GetVariable("isAssassinated").GetValue() && !(bool)behaviourTree.GetVariable("isParried").GetValue())
            animator.SetLayerWeight(1, 1);
    }
}
