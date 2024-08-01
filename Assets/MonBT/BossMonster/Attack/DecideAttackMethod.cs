using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class DecideAttackMethod : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    private int AttackTypeIndex;
    private int AttackIndex;
    private int maxAttackIndex;

    private Transform traceTarget;
    private bool isAction;

    private int hashAttack = Animator.StringToHash("Attack");
    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");
    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = monsterView.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        traceTarget = monsterView.vm.TraceTarget;
        AttackTypeIndex = Random.Range(0, monsterView.AttackMethodCount);
        switch (AttackTypeIndex)
        {
            case 0:
                agent.stoppingDistance = 2.5f;
                maxAttackIndex = 2;
                break;
            case 1:
                agent.stoppingDistance = 2f;
                maxAttackIndex = 3; 
                break;
            case 2:
                agent.stoppingDistance = 2.8f;
                maxAttackIndex = 1; 
                break;
        }

        AttackIndex = Random.Range(0, maxAttackIndex);
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

                animator.SetInteger(hashAttackTypeIndex, AttackTypeIndex);
                animator.SetInteger(hashAttackIndex, AttackIndex);
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
