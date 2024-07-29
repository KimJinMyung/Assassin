using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.iOS;

[TaskCategory("Battle")]
public class CheckTraceTaretRangeOnUpdate : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedFloat AttackRange;
    [SerializeField] SharedBool isAttackAble;

    private float distance;
    private int hashMovespeed = Animator.StringToHash("MoveSpeed");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        if(monsterView.vm.TraceTarget == null) return TaskStatus.Failure;
        distance = Vector3.Distance(Owner.transform.position, monsterView.vm.TraceTarget.position);
        AttackRange.Value = monsterView.vm.CurrentAttackMethod.AttackRange;

        agent.speed = monsterView._monsterData.RunSpeed;
        agent.stoppingDistance = AttackRange.Value + 1.5f;

        if (distance >= AttackRange.Value + 1.5f)
        {
            isAttackAble.Value = false;
            animator.SetFloat(hashMovespeed, 1);
            return TaskStatus.Running;
        }

        isAttackAble.Value = true;
        animator.SetFloat(hashMovespeed, 0);
        return TaskStatus.Success;
    }
}
