using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Attack")]
public class CheckisAttackAbleDisOnUpdate : Action
{
    private MonsterView monsterView;
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] SharedFloat AttackRange;
    [SerializeField] SharedString AttackName;

    int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedFloat AttackIndex;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        agent = Owner.GetComponent<NavMeshAgent>();
        animator = Owner.GetComponent<Animator>();        
    }

    public override void OnStart()
    {
        AttackRange.Value = monsterView.vm.CurrentAttackMethod.AttackRange;
        AttackName.Value = monsterView.vm.CurrentAttackMethod.DataName;

        agent.speed = monsterView._monsterData.RunSpeed;
        agent.stoppingDistance = AttackRange.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.vm.TraceTarget == null) return TaskStatus.Failure;
        float distance = Vector3.Distance(transform.position, monsterView.vm.TraceTarget.position);
        if (AttackRange.Value + 0.3f >= distance)
        {
            isAttackAble.Value = false;
            animator.SetFloat(hashMoveSpeed, 0f);
            animator.SetTrigger($"{AttackName.Value}");

            return TaskStatus.Success;
        }

        animator.SetFloat (hashMoveSpeed, 1f);
        agent.SetDestination(monsterView.vm.TraceTarget.position);
        return TaskStatus.Running;
    }    
}
