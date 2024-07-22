using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackBox : StateMachineBehaviour
{
    AttackBox attackBox;

    [SerializeField] private float AttackBoxOnTime;
    [SerializeField] private float AttackBoxOffTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackBox = animator.GetComponentInChildren<AttackBox>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= AttackBoxOnTime && stateInfo.normalizedTime <= AttackBoxOffTime) 
        {
            attackBox.enabled = true;
        }
        else attackBox.enabled = false;
    }
}