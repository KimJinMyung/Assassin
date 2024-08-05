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

    private string attackName;
    private string attackType;

    private int currentAttackTypeIndex;
    private int currentAttackIndex;


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
        currentAttackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
        currentAttackIndex = animator.GetInteger(hashAttackIndex);

        traceTarget = monsterView.vm.TraceTarget;

        attackName = monsterView.vm.CurrentAttackMethod.DataName;
        switch (currentAttackTypeIndex)
        {
            case 0:
                attackType = "JumpAttack";
                agent.stoppingDistance = 5f;
                break;
            case 1:
                attackType = "ComBoAttack";
                agent.stoppingDistance = 4f;
                break;
            case 2:
                attackType = "DashAttack";
                agent.stoppingDistance = 8f;
                break;
        }
    }

    public override TaskStatus OnUpdate()
    {        
        if(traceTarget == null) return TaskStatus.Failure;
        if (isAction && monsterView.IsAnimationRunning($"{attackType}.attack0{currentAttackIndex}"))
        {
            return TaskStatus.Success;
        }

        agent.SetDestination(traceTarget.position);

        float distance = Vector3.Distance(Owner.transform.position, traceTarget.position);
        //Debug.Log(distance);
        if(distance <= agent.stoppingDistance)
        {
            if (!isAction)
            {
                isAttacking.Value = true;
                isAttackAble.Value = false;

                animator.SetTrigger(hashAttack);

                isAction = true;
            }            
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
