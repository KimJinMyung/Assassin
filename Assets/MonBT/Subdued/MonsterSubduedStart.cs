using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Subdued")]
public class MonsterSubduedStart : Action
{
    private Animator animator;

    [SerializeField] SharedBool isSubed;
    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    private int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashIncapacitate = Animator.StringToHash("Incapacitate");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isSubed.Value) return TaskStatus.Failure;

        isAttackAble.Value = false;
        isAttacking.Value = false;
        animator.SetBool(hashIncapacitated, true);
        animator.SetTrigger(hashIncapacitate);
        return TaskStatus.Success;
    }
}
