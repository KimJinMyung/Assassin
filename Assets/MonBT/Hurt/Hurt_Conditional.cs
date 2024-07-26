using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Hurt")]
public class Hurt_Conditional : Conditional
{
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;

    public override TaskStatus OnUpdate()
    {
        if(!isHurt.Value || isDead.Value) return TaskStatus.Failure;
        return TaskStatus.Success;
    }
}
