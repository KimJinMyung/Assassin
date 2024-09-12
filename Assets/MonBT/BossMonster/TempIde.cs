using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Temp")]
public class TempIde : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
