using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Attack")]
public class CompleteAttackAnimation : Conditional
{
    private MonsterView monsterView;

    [SerializeField] SharedFloat AttackIndex;
    [SerializeField] SharedString AttackName;
    [SerializeField] SharedBool isAttackEnd;

    private bool isMotionOn;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(isAttackEnd.Value) return TaskStatus.Success;

        if (monsterView.IsAnimationRunning($"{AttackName.Value}.attack{AttackIndex.Value}"))
        {
            isMotionOn =true;
            return TaskStatus.Running;
        }

        if (isMotionOn)
        {
            isAttackEnd.Value = true;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isMotionOn = false;
    }
}
