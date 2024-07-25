using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Trace")]
public class CheckTraceTaretRangeOnUpdate : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedFloat AttackRange;
    [SerializeField] SharedBool isCircling;
    [SerializeField] SharedBool isAttackAble;

    private float distance;

    public override void OnStart()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        distance = Vector3.Distance(Owner.transform.position, monsterView.vm.TraceTarget.position);
        AttackRange.Value = monsterView.vm.CurrentAttackMethod.AttackRange;

        agent.speed = monsterView._monsterData.RunSpeed;
        agent.stoppingDistance = AttackRange.Value + 1.5f;

        if (distance >= AttackRange.Value + 1.5f)
        {
            isCircling = false;
            isAttackAble = false;
            animator.SetFloat("MoveSpeed", 1);
            return TaskStatus.Running;
        }

        isAttackAble = true;
        animator.SetFloat("MoveSpeed", 0);
        return TaskStatus.Success;
    }
}
