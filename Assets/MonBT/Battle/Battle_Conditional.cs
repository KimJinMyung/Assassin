using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Battle")]
public class Battle_Conditional : Conditional
{
    MonsterView monsterView;

    [SerializeField] SharedBool isCircling;
    [SerializeField] SharedBool isAttacking;
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;

    private float distance;
    [SerializeField] SharedFloat AttackRange;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override void OnStart()
    {
        distance = Vector3.Distance(transform.position, monsterView.vm.TraceTarget.position);
    }

    public override TaskStatus OnUpdate()
    {
        if (isHurt.Value || isDead.Value || isAttacking.Value) return TaskStatus.Failure;
        if(monsterView.vm.TraceTarget == null) return TaskStatus.Failure;
        if (distance > AttackRange.Value + 1.5f) return TaskStatus.Failure;

        return TaskStatus.Success;

    }
}
