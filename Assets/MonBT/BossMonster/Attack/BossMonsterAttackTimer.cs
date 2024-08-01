using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossMonsterAttackTimer : Conditional
{
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isAttacking;

    [SerializeField] SharedFloat timer;
    [SerializeField] SharedBool isAttackAble;

    public override TaskStatus OnUpdate()
    {       
        if(isDead.Value || isHurt.Value || isAttacking.Value) return TaskStatus.Failure;

        timer.Value = Mathf.Clamp(timer.Value - Time.deltaTime, 0, 5);

        if(timer.Value <= 0 ) isAttackAble.Value = true;
        else isAttackAble.Value = false;

        return TaskStatus.Running;
    }
}
