using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossDash")]
public class DashStart : Action
{
    private MonsterView monsterView;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Animator animtor;

    [SerializeField] private float dashPower;

    private int attackIndex;

    private int hashAttackIndex = Animator.StringToHash("AttackIndex");
    private int hashNextAction = Animator.StringToHash("NextAction");

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
        if (monsterView.IsAnimationRunning($"DashAttack.attack0{attackIndex}"))
        {
            return TaskStatus.Running;
        }

        rb.mass = 1f;
        rb.isKinematic = false;
        agent.enabled = false;
        rb.AddForce(Owner.transform.forward * dashPower, ForceMode.Impulse);
        animtor.SetTrigger(hashNextAction);

        if (monsterView.IsAnimationRunning($"DashAttack.attack1{attackIndex}"))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            var target = other.GetComponent<PlayerView>();
            if (target == null) return;

            target.Hurt(monsterView, monsterView._monsterData.ATK);
            //EventManager<PlayerAction>.TriggerEvent(PlayerAction.KnockBack, Owner.transform.position);
            Debug.Log("Monster Attack");
        }
    }
}
