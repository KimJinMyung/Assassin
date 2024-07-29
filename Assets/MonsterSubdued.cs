using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class MonsterSubdued : StateMachineBehaviour
{
    BehaviorTree behaviourTree;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        behaviourTree = animator.GetComponent<BehaviorTree>();

        if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.layerCount <= 1) return;
        if (!(bool)behaviourTree.GetVariable("isDead").GetValue() && !(bool)behaviourTree.GetVariable("isAssassinated").GetValue() && !(bool)behaviourTree.GetVariable("isParried").GetValue())
            animator.SetLayerWeight(1, 1);
    }
}
