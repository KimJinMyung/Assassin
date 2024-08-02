using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class JumpAttack_Conditional : Conditional
{
    private Animator animator;

    private int attackTypeIndex;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        attackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
    }

    public override TaskStatus OnUpdate()
    {
        if (attackTypeIndex == 0) return TaskStatus.Success;
        else return TaskStatus.Failure;
    }
}
