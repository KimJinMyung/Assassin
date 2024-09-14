using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParried : StateMachineBehaviour
{
    private MonsterView monsterView;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterView = animator.GetComponent<MonsterView>();

        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.AttackColliderOn, monsterView.monsterId, false);
        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.JumpAttackColliderOn, monsterView.monsterId, false);
    }
}
