using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Monster_Patrol")]
public class PatrolMoveOnUpdate : Action
{
    private MonsterView monsterView;
    private Animator _animator;
    private NavMeshAgent agent;

    private Vector3 patrolPos;
    private float moveSpeed;

    public override void OnAwake()
    {
        base.OnAwake();
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();
        agent = monsterView.GetComponent <NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.isDead || monsterView.isHurt) return TaskStatus.Failure;
        if (monsterView.vm.TraceTarget != null) return TaskStatus.Failure;
        if (monsterView.isAttacking) return TaskStatus.Failure;

        agent.stoppingDistance = 0.1f;
        agent.speed = monsterView._monsterData.WalkSpeed;

        monsterView.MoveToTarget(patrolPos);

        moveSpeed = Vector3.Distance(transform.position, patrolPos) > 0.1f ? 1 : 0;

        _animator.SetFloat("MoveSpeed", moveSpeed);

        if (agent.remainingDistance > 0.1f) return TaskStatus.Running;

        isPatrol = false;
        return TaskStatus.Success;
    }


}
