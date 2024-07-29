using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Parried")]
public class CompleteParriedAnimation : Action
{
    MonsterView monsterView;
    Animator animator;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isSubdued;
    [SerializeField] SharedBool isAttackAble;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsAnimationRunning("Parried"))
        {
            return TaskStatus.Running;
        }

        isParried.Value = false;
        isSubdued.Value = monsterView.vm.Stamina <= 0;
        if (animator.layerCount > 1) animator.SetLayerWeight(1, 1);
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        isAttackAble.Value = true;
    }
}
