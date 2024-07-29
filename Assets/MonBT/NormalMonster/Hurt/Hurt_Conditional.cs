using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Hurt")]
public class Hurt_Conditional : Conditional
{
    private AttackBox _attackBox;

    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;

    public override void OnAwake()
    {
        _attackBox = Owner.GetComponentInChildren<AttackBox>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isHurt.Value || isDead.Value || isAssassinated.Value) return TaskStatus.Failure;
        _attackBox.enabled = false;
        return TaskStatus.Success;
    }
}
