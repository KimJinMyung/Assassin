using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Parried")]
public class MonsterParriedAnimation : Action
{
    private MonsterView monsterView;
    private Animator animator;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    private int hashParried = Animator.StringToHash("Parried");
    private bool isAction;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!isParried.Value) return TaskStatus.Failure;

        if (!isAction)
        {
            if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);
            isAttacking.Value = false;
            isAttackAble.Value = false;
            isAction = true;
            animator.SetTrigger(hashParried);      
        }

        if (monsterView.IsAnimationRunning("Parried")) return TaskStatus.Success;
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
