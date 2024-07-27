using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Parried")]
public class Parried_Conditional : Conditional
{

    [SerializeField] SharedBool isParried;

    public override TaskStatus OnUpdate()
    {
        if(!isParried.Value) return TaskStatus.Failure;
        return TaskStatus.Success;
    }
}
