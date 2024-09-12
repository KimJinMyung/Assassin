using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Subdued")]
public class MonsterSubduedStart : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private Rigidbody rb;
    private NavMeshAgent agent;

    [SerializeField] SharedBool isSubed;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAttackAble;
    [SerializeField] SharedBool isAttacking;

    private readonly int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private readonly int hashIncapacitate = Animator.StringToHash("Incapacitate");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashNextAction = Animator.StringToHash("NextAction");
    private readonly int hashJump = Animator.StringToHash("Jump");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        rb = Owner.GetComponent<Rigidbody>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        agent.ResetPath();

        if(monsterView.Type == MonsterType.Boss)
        {
            animator.ResetTrigger(hashAttack);
            animator.ResetTrigger(hashNextAction);
            animator.ResetTrigger(hashJump);
        }

        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, monsterView.monsterId, false);
    }

    public override TaskStatus OnUpdate()
    {
        if(!isSubed.Value || isDead.Value) return TaskStatus.Failure;

        isAttackAble.Value = false;
        isAttacking.Value = false;
        animator.SetBool(hashIncapacitated, true);
        animator.SetTrigger(hashIncapacitate);
        if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);
        return TaskStatus.Success;
    }
}
