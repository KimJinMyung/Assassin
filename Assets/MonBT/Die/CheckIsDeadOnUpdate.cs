using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Monster_Die")]
public class CheckIsDeadOnUpdate : Action
{
    private MonsterView monsterView;
    private Animator _animator;

    int hashDead = Animator.StringToHash("Dead");
    int hashDie = Animator.StringToHash("Die");
    int hashIncapacitated = Animator.StringToHash("Incapacitated");

    public override void OnAwake()
    {
        base.OnAwake();
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_animator.GetBool(hashDead))
        {
            if (_animator.GetBool(hashIncapacitated))
            {
                monsterView.MonsterDead();
            }
            return TaskStatus.Success;
        }
        if (!monsterView.isDead) return TaskStatus.Failure;

        monsterView.MonsterDead();
        _animator.SetBool(hashDead, true);
        _animator.SetTrigger(hashDie);
        return TaskStatus.Success;
    }
}
