using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossBackJump")]
public class Backjump_Conditional : Conditional
{
    private Animator animator;

    [SerializeField] SharedBool isAttacking;

    private int AttackTypeIndex;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        AttackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
    }

    public override TaskStatus OnUpdate()
    {
        if(isAttacking.Value) return TaskStatus.Failure;

        if(AttackTypeIndex == 1) return TaskStatus.Failure;
        else return TaskStatus.Success;
    }
}
