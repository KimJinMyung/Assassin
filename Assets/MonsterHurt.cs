using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHurt : StateMachineBehaviour
{
    private MonsterView monsterView;

    private string AttackName;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterView = animator.GetComponent<MonsterView>();
        AttackName = monsterView.vm.CurrentAttackMethod.DataName;

        animator.ResetTrigger(AttackName);
    }
}
