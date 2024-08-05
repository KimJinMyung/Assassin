using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class DecideBossAttackIndex : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    private int AttackTypeIndex;
    private int AttackIndex;
    private int maxAttackIndex;

    private Transform traceTarget;

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
        AttackTypeIndex = 0;//Random.Range(0, monsterView.AttackMethodCount);
        switch (AttackTypeIndex)
        {
            case 0:
                maxAttackIndex = 2;
                break;
            case 1:
                maxAttackIndex = 3; 
                break;
            case 2:
                maxAttackIndex = 1; 
                break;
        }

        AttackIndex = Random.Range(0, maxAttackIndex);
    }

    public override TaskStatus OnUpdate()
    {        
        if(traceTarget == null)
            return TaskStatus.Failure;

        animator.SetInteger(hashAttackTypeIndex, AttackTypeIndex);
        animator.SetInteger(hashAttackIndex, AttackIndex);

        return TaskStatus.Success;
    }
}
