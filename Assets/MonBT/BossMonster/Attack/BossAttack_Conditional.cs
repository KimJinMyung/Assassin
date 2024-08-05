using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class BossAttack_Conditional : Conditional
{
    private NavMeshAgent agent;

    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;
    [SerializeField] SharedBool isDie;
    [SerializeField] SharedBool isParried;

    public override void OnAwake()
    {
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        if(isDie.Value || isParried.Value) return TaskStatus.Failure;

        if(isAttackAble.Value || isAttacking.Value)
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }

    public override void OnEnd()
    {
        agent.enabled = true;
    }
}
