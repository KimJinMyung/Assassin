using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossMonsterBackJump : Action
{
    private Animator animator;

    private int attackTypeIndex;

    private bool isAction;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();        
    }

    public override void OnStart()
    {
        attackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
    }

    //public override TaskStatus OnUpdate()
    //{
    //    if(attackTypeIndex != 1) return TaskStatus.Success;

    //    if (!isAction)
    //    {

    //    }
    //}
}
