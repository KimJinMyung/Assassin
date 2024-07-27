using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Parried")]
public class MonsterParriedAnimation : Action
{
    private Animator animator;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    int hashParried = Animator.StringToHash("Parried");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!isParried.Value) return TaskStatus.Failure;

        if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);
        isAttacking.Value = false;
        isAttackAble.Value = false;
        animator.SetTrigger(hashParried);
        return TaskStatus.Success;
    }
}
