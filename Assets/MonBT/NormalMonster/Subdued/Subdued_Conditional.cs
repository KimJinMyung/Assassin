using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Subdued")]
public class Subdued_Conditional : Conditional
{
    private AttackBox_Monster _attackBox;

    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isSubded;
    [SerializeField] SharedBool isAssassinated;


    public override void OnAwake()
    {
        _attackBox = Owner.GetComponentInChildren<AttackBox_Monster>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isSubded.Value || isHurt.Value || isDead.Value || isAssassinated.Value) return TaskStatus.Failure;
        _attackBox.enabled = false;
        return TaskStatus.Running;
    }
}
