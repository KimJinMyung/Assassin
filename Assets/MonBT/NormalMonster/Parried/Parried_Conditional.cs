using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[TaskCategory("Parried")]
public class Parried_Conditional : Conditional
{
    private Animator animator;
    private AttackBox_Monster _attackBox;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAssassinated;

    private int hashJump = Animator.StringToHash("Jump");
    private int hashAttack = Animator.StringToHash("Attack");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
        _attackBox = Owner.GetComponentInChildren<AttackBox_Monster>();
    }

    public override void OnStart()
    {
        //animator.ResetTrigger(hashJump);
        //animator.ResetTrigger(hashAttack);
    }

    public override TaskStatus OnUpdate()
    {
        if(!isParried.Value || isAssassinated.Value) return TaskStatus.Failure;

        _attackBox.enabled = false;
        return TaskStatus.Running;
    }
}
