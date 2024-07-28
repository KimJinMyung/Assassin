using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Assassinated")]
public class Assassinated_Conditional : Conditional
{
    private AttackBox attackBox;

    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;

    public override void OnAwake()
    {
        attackBox = Owner.GetComponentInChildren<AttackBox>();
    }

    public override TaskStatus OnUpdate()
    {
        if (isDead.Value || !isAssassinated.Value) return TaskStatus.Failure;
        attackBox.enabled = false;
        return TaskStatus.Success;
    }
}
