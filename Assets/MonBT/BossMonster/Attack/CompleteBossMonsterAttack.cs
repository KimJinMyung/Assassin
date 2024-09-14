using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("BossAttack")]
public class CompleteBossMonsterAttack : Conditional
{
    private MonsterView monsterView;
    private Animator animator;
    private NavMeshAgent agent;

    private int currentAttackTypeIndex;
    private int currentAttackIndex;

    private bool isMotionOn;
    private string attackName;
    private string attackType;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");
    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        currentAttackTypeIndex = animator.GetInteger(hashAttackTypeIndex);
        currentAttackIndex = animator.GetInteger(hashAttackIndex);

        attackName = monsterView.vm.CurrentAttackMethod.DataName;
        switch (currentAttackTypeIndex)
        {
            case 0:
                attackType = "JumpAttack";
                break;
            case 1:
                attackType = "ComBoAttack";
                break;
            case 2:
                attackType = "DashAttack";
                break;
        }

        agent.stoppingDistance = 1f;

        if(attackType == "JumpAttack")
        {
            EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.JumpAttackColliderOn, monsterView.monsterId, true);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsAnimationRunning($"{attackType}.attack0{currentAttackIndex}"))
        {
            //isMotionOn = true;
            return TaskStatus.Running;
        }

        //if(isMotionOn)
            return TaskStatus.Success;

        //return TaskStatus.Running;
    }    
}
