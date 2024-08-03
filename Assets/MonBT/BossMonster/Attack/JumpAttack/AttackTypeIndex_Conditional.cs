using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class AttackTypeIndex_Conditional : Conditional
{
    private Animator animator;

    [SerializeField] private int AttackTypeIndex;

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
        if (attackTypeIndex == AttackTypeIndex) return TaskStatus.Success;
        else return TaskStatus.Failure;
    }
}
