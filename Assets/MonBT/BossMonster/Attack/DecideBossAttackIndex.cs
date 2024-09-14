using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class DecideBossAttackIndex : Action
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedBool DebugMode;

    private int AttackTypeIndex;
    private int AttackIndex;
    private int maxAttackIndex;

    private int AttackCount;

    private Transform traceTarget;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");
    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = monsterView.GetComponent<NavMeshAgent>();


        if(monsterView.Type == MonsterType.Boss)
        {
            animator.SetInteger(hashAttackTypeIndex, 1);
        }

        AttackCount = 0;
    }

    public override void OnStart()
    {
        animator.ResetTrigger("NextAction");

        traceTarget = monsterView.vm.TraceTarget;

        if (DebugMode.Value) return;

        if(AttackCount <= 3)
        {
            var GetAttackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
            AttackTypeIndex = Mathf.Abs((GetAttackTypeIndex + 1) % 3);
            AttackCount++;
        }
        else
        {
            AttackTypeIndex = Random.Range(0, 3);
        }

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

        animator.SetInteger(hashAttackTypeIndex, AttackTypeIndex);
        animator.SetInteger(hashAttackIndex, AttackIndex);
    }

    public override TaskStatus OnUpdate()
    {        
        if(traceTarget == null)
            return TaskStatus.Failure;
        else 
            return TaskStatus.Success;
    }
}
