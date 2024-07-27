using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Subdued")]
public class MonsterSubduedEnd : Action
{
    private Animator animator;

    [SerializeField] SharedBool isSubded;
    [SerializeField] SharedBool isAttackAble;

    private int hashIncapacitated = Animator.StringToHash("Incapacitated");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        animator.SetBool(hashIncapacitated, false);
        isSubded.Value = false;
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        isAttackAble.Value = true;
    }
}
