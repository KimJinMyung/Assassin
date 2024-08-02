using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Battle")]
public class Battle_Conditional : Conditional
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedBool isCircling;
    [SerializeField] SharedBool isAttacking;
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;

    [SerializeField] SharedBool isAttackAble;

    private float distance;
    [SerializeField] SharedFloat AttackRange;

    private int hashMovespeed = Animator.StringToHash("MoveSpeed");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = monsterView.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        if(monsterView.vm.TraceTarget != null)        
        AttackRange.Value = monsterView.vm.CurrentAttackMethod.AttackRange;

        agent.stoppingDistance = AttackRange.Value + 1.5f;
        agent.speed = monsterView._monsterData.RunSpeed;
    }

    public override TaskStatus OnUpdate()
    {
        if (isHurt.Value || isDead.Value || isAttacking.Value || isAssassinated.Value) return TaskStatus.Failure;
        if(monsterView.vm.TraceTarget == null) return TaskStatus.Failure;

        distance = Vector3.Distance(monsterView.transform.position, monsterView.vm.TraceTarget.position);

        agent.SetDestination(monsterView.vm.TraceTarget.position);

        if (distance > AttackRange.Value + 1.5f)
        {
            if(monsterView.Type != MonsterType.Boss) isAttackAble.Value = false;
            animator.SetFloat(hashMovespeed, 1);
        }
        else
        {
            if (monsterView.Type != MonsterType.Boss) isAttackAble.Value = true;
            animator.SetFloat(hashMovespeed, 0);            
        }

        return TaskStatus.Running;
    }
}
