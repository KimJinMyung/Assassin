using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAssassinatedEnd : StateMachineBehaviour
{
    private BehaviorTree tree;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tree = animator.GetComponent<BehaviorTree>();

        tree.SetVariableValue("isSubded", false);
        tree.SetVariableValue("isDead", false);
        tree.SetVariableValue("isAssassinated", false);
    }
}
