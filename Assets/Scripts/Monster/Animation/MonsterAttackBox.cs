using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackBox : StateMachineBehaviour
{
    MonsterView monsterVIew;
    AttackBox attackBox;

    [SerializeField] private float AttackBoxOnTime;
    [SerializeField] private float AttackBoxOffTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterVIew = animator.GetComponent<MonsterView>();
        attackBox = monsterVIew.attackBox;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= AttackBoxOnTime && stateInfo.normalizedTime <= AttackBoxOffTime) 
        {
            attackBox.enabled = true;
        }
        else attackBox.enabled = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackBox.enabled = false;
    }
}
