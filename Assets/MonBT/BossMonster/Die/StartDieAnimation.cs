using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossDie")]
public class StartDieAnimation : Action
{
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;
    [SerializeField] SharedBool isDone;

    private MonsterView monsterView;
    private Animator animator;

    private int instanceID;

    private bool isAction;

    private int hashDead = Animator.StringToHash("Dead");
    private int hashDie = Animator.StringToHash("Die");
    private int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashUpper = Animator.StringToHash("Upper");
    private int Assassinated = Animator.StringToHash("Assassinated");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();

        instanceID = monsterView.GetInstanceID();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isDead.Value) return TaskStatus.Failure;
        if(isDone.Value) return TaskStatus.Failure;

        var lifeCount = monsterView.vm.LifeCount - 1;
        monsterView.vm.RequestMonsterLifeCountChanged(lifeCount, instanceID);

        var IsDead = monsterView.vm.LifeCount <= 0;

        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.IsDead, IsDead, instanceID);

        if (IsDead)
        {
            monsterView.MonsterDead();
        }

        if(!isAction)
        {
            monsterView.BossMonsterDead();

            if (isAssassinated.Value)
            {
                animator.SetBool(hashIncapacitated, true);
                animator.SetTrigger(Assassinated);
            }
            else
            {
                animator.SetBool(hashDead, true);
                animator.SetTrigger(hashDie);
            }

            isAction = true;
            isDone.Value = true;
        }

        if (!animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("Assassinated")) return TaskStatus.Success;
        else if (animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Success;
        else if (monsterView.IsAnimationRunning("Die")) return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
