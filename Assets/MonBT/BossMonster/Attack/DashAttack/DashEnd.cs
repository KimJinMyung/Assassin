using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossDash")]
public class DashEnd : Action
{
    private MonsterView monsterView;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator animtor;

    private int attackIndex;

    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        rb = Owner.GetComponent<Rigidbody>();
        agent = Owner.GetComponent<NavMeshAgent>();
        animtor = Owner.GetComponent<Animator>();
    }
    public override void OnStart()
    {
        attackIndex = animtor.GetInteger(hashAttackIndex);
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsAnimationRunning($"DashAttack.attack1{attackIndex}"))
        {
            var distance = Vector3.Distance(Owner.transform.position, monsterView.vm.TraceTarget.position);
            if (distance <= 15f)
            {
                rb.velocity = Vector3.zero;
            }

            return TaskStatus.Running;
        }        

        rb.isKinematic = true;
        agent.enabled = true;
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.DashAttack, monsterView.monsterId, false);
    }
}
