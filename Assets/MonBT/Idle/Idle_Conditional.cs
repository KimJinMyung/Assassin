using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Idle")]
public class Idle_Conditional : Conditional
{
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAttacking;

    MonsterView monsterView;

    public override void OnStart()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(monsterView.vm.TraceTarget !=null || isHurt.Value || isDead.Value || isAttacking.Value) return TaskStatus.Failure;
        else return TaskStatus.Success;
    }
}
