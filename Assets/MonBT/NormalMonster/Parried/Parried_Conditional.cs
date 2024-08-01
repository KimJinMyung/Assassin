using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Parried")]
public class Parried_Conditional : Conditional
{

    private AttackBox _attackBox;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAssassinated;


    public override void OnAwake()
    {
        _attackBox = Owner.GetComponentInChildren<AttackBox>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isParried.Value || isAssassinated.Value) return TaskStatus.Failure;

        _attackBox.enabled = false;
        return TaskStatus.Running;
    }
}
