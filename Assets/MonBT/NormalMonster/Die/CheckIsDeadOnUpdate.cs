using BehaviorDesigner.Runtime;
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

    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isDIsable;
    [SerializeField] SharedBool isAssassinated;

    private int hashDead = Animator.StringToHash("Dead");
    private int hashDie = Animator.StringToHash("Die");
    private int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashUpper = Animator.StringToHash("Upper");
    private int Assassinated = Animator.StringToHash("Assassinated");

    private bool isAction;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!isDead.Value) return TaskStatus.Failure;
        if(isDIsable.Value) return TaskStatus.Success;

        //동작 실행
        if (!isAction)
        {
            monsterView.MonsterDead();

            if (isAssassinated.Value)
            {
                _animator.SetBool(hashIncapacitated, true);
                _animator.SetTrigger(Assassinated);               
            }
            else
            {
                _animator.SetBool(hashDead, true);
                _animator.SetTrigger(hashDie);                
            }
            isAction = true;
            isDIsable.Value = true;
        }

        //해당 동작들이 실행되고 있는지 확인
        if (!_animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("Assassinated")) return TaskStatus.Success;
        else if (_animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Success;
        else if (monsterView.IsAnimationRunning("Die")) return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isAction = false;
    }
}
