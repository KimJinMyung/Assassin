using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossAttack_Conditional : Conditional
{
    [SerializeField] SharedBool isAttackAble;

    public override TaskStatus OnUpdate()
    {
        if(isAttackAble.Value)
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
