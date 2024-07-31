using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Attack")]
public class RetreatAfterAttack : Action
{
    MonsterView monsterView;
    NavMeshAgent agent;
    Animator animator;

    [SerializeField] SharedBool isAttackEnd;
    [SerializeField] SharedBool isAttacking;


    int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    private float AttackRange;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        agent = Owner.GetComponent<NavMeshAgent>();
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        agent.speed = monsterView._monsterData.WalkSpeed;
        animator.SetFloat(hashMoveSpeed, -1);

        AttackRange = monsterView.vm.CurrentAttackMethod.AttackRange;
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.vm.CurrentAttackMethod.AttackType == "Long")
        {
            isAttackEnd.Value = false;
            isAttacking.Value = false;
            return TaskStatus.Success;
        }

        float distance = Vector3.Distance(Owner.transform.position, monsterView.vm.TraceTarget.position);
        if(distance >= AttackRange + 1.5f)
        {
            isAttackEnd.Value = false;
            isAttacking.Value = false;
            return TaskStatus.Success;
        }

        Vector3 targetDir = monsterView.vm.TraceTarget.position - Owner.transform.position;
        targetDir.y = 0;

        agent.Move(-targetDir.normalized * (AttackRange + 1.5f) * Time.deltaTime);

        Owner.transform.rotation = Quaternion.RotateTowards(Owner.transform.rotation, Quaternion.LookRotation(targetDir), 500 * Time.deltaTime);
        return TaskStatus.Running;
    }
}
