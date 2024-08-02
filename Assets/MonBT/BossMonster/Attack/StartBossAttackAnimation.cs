using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class StartBossAttackAnimation : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    private Transform traceTarget;
    private bool isAction;

    private int hashAttack = Animator.StringToHash("Attack");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = monsterView.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        traceTarget = monsterView.vm.TraceTarget;
    }

    public override TaskStatus OnUpdate()
    {        
        if(traceTarget == null) return TaskStatus.Failure;
        float distance = Vector3.Distance(Owner.transform.position, traceTarget.position);
        if(distance <= agent.stoppingDistance)
        {
            if (!isAction)
            {
                isAttacking.Value = true;
                isAttackAble.Value = false;

                animator.SetTrigger(hashAttack);

                isAction = true;
                return TaskStatus.Success;
            }            
        }

        agent.SetDestination(traceTarget.position);
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
