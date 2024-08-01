using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Attack")]
public class Attack_Conditional : Conditional
{
    private MonsterView monsterView;    

    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAttacking;
    [SerializeField] SharedBool isAssassinated;


    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(isDead.Value || isHurt.Value || isParried.Value || !isAttacking.Value || monsterView.vm.TraceTarget == null || isAssassinated.Value) return TaskStatus.Failure;
        else return TaskStatus.Running;
    }
}
