using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Idle")]
public class MoveToPoint : Action
{
    MonsterView monsterView;
    Animator animator;
    NavMeshAgent agent;

    [SerializeField] SharedVector3 patrolPos;
    [SerializeField] SharedFloat moveSpeed;
    [SerializeField] SharedBool isPatrol;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        agent.stoppingDistance = 0.1f;
        agent.speed = monsterView._monsterData.WalkSpeed;

        agent.SetDestination(patrolPos.Value);

        moveSpeed.Value = Vector3.Distance(Owner.transform.position, patrolPos.Value) > 0.1f ? 1 : 0;

        animator.SetFloat("MoveSpeed", moveSpeed.Value);

        if (agent.remainingDistance > 0.1f) return TaskStatus.Running;

        isPatrol.Value = false;
        return TaskStatus.Success;
    }
}
