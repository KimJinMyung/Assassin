using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Battle")]
public class ChangeCirclingDelayTime : Action
{
    [SerializeField] SharedFloat CirclingDelayTime;
    public override void OnStart()
    {
        if(CirclingDelayTime.Value <= 0)
        CirclingDelayTime.Value = Random.Range(1.5f, 3f);
    }

    public override TaskStatus OnUpdate()
    {
        if (CirclingDelayTime.Value > 0)
        {
            return TaskStatus.Success;
        }
        else return TaskStatus.Failure;
    }
}
