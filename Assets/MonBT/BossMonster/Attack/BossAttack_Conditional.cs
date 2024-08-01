using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossAttack_Conditional : Conditional
{
    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    public override TaskStatus OnUpdate()
    {
        if(isAttackAble.Value || isAttacking.Value)
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}