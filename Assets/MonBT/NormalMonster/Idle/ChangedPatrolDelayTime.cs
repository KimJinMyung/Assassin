using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Idle")]
public class ChangedPatrolDelayTime : Action
{
    [SerializeField] SharedFloat PatrolTime;

    public override void OnStart()
    {
        PatrolTime.Value = Random.Range(1.5f, 3f);
    }

    public override TaskStatus OnUpdate()
    {
        if (PatrolTime.Value > 0) return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}
