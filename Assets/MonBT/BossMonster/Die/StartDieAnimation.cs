using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[TaskCategory("BossDie")]
public class StartDieAnimation : Action
{
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;
    [SerializeField] SharedBool isNextAction;
    
    private MonsterView monsterView;
    private Animator animator;
    private Rigidbody rb;

    private int instanceID;

    private bool isAction;

    private int hashDead = Animator.StringToHash("Dead");
    private int hashDie = Animator.StringToHash("Die");
    private int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashUpper = Animator.StringToHash("Upper");
    private int Assassinated = Animator.StringToHash("Assassinated");
    private int hashAttack = Animator.StringToHash("Attack");
    private int hashJump = Animator.StringToHash("Jump");
    private int hashDefense = Animator.StringToHash("Defence");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        rb = Owner.GetComponent <Rigidbody>();

        instanceID = monsterView.GetInstanceID();
    }

    public override void OnStart()
    {
        base.OnStart();

        isAction = false;

        animator.ResetTrigger(hashAttack);
        animator.ResetTrigger(hashJump);
        animator.ResetTrigger(hashDefense);
        rb.useGravity = true;        
    }

    public override TaskStatus OnUpdate()
    {
        if(!isDead.Value) return TaskStatus.Failure;

        if (!isAction)
        {
            if (!isAssassinated.Value)
            {
                animator.SetBool(hashDead, true);
                animator.SetTrigger(hashDie);
            }
            else
            {
                animator.SetBool(hashDead, true);
                animator.SetTrigger(Assassinated);
            }
            isAction = true;
        }

        var lifeCount = monsterView.vm.LifeCount;

        if (lifeCount-1 > 0)
        {
            return TaskStatus.Success;
        }
        else
        {
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.AttackLockOnEnable, this);
            Owner.gameObject.SetActive(false);
        }
            
        //if(isDisable.Value) return TaskStatus.Failure;

        //var lifeCount = monsterView.vm.LifeCount - 1;
        //monsterView.vm.RequestMonsterLifeCountChanged(lifeCount, instanceID);

        //var IsDead = monsterView.vm.LifeCount <= 0;

        //EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.IsDead, IsDead, instanceID);

        //if (IsDead)
        //{
        //    monsterView.MonsterDead();
        //}

        //if (!isAction)
        //{
        //    monsterView.BossMonsterDead();

        //    if (isAssassinated.Value)
        //    {
        //        animator.SetBool(hashIncapacitated, true);
        //        animator.SetTrigger(Assassinated);
        //    }
        //    else
        //    {
        //        animator.SetBool(hashDead, true);
        //        animator.SetTrigger(hashDie);
        //    }

        //    isAction = true;
        //    //isDisable.Value = true;
        //}

        //if (!animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("Assassinated")) return TaskStatus.Success;
        //else if (animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Success;
        //else if (monsterView.IsAnimationRunning("Die")) return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
