using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using static UnityEngine.UI.GridLayoutGroup;
using System;

public class MonsterAttack : StateMachineBehaviour
{
    private MonsterView monsterView;
    private BehaviorTree behaviourTree;

    private string AttackName;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterView = animator.GetComponent<MonsterView>();
        behaviourTree = animator.GetComponent<BehaviorTree>();

        AttackName = monsterView.vm.CurrentAttackMethod.DataName;

        animator.applyRootMotion = true;
        if (animator.layerCount >= 2)
            animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(AttackName);

        animator.applyRootMotion = false;
        if (animator.layerCount <= 1) return;
        if(!(bool)behaviourTree.GetVariable("isDead").GetValue() && !(bool)behaviourTree.GetVariable("isAssassinated").GetValue() && !(bool)behaviourTree.GetVariable("isParried").GetValue() && !(bool)behaviourTree.GetVariable("isSubded").GetValue())
            animator.SetLayerWeight(1, 1);
    }
}
