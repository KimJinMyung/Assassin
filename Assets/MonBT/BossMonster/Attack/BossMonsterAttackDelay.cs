using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossMonsterAttackDelay : Action
{
    [SerializeField] SharedFloat AttackTimer;
    [SerializeField] SharedBool isAttacking;

    public override void OnStart()
    {
        AttackTimer.Value = Random.Range(1f, 2.5f);
    }

    public override TaskStatus OnUpdate()
    {
        isAttacking.Value = false;
        return TaskStatus.Success;
    }
}
