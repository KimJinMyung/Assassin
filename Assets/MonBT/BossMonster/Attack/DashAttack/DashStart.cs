using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using Player;
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
        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.DashAttack, monsterView.monsterId, true);
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
            rb.useGravity = true;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
