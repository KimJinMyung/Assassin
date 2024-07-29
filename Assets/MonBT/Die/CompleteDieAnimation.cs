using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Monster_Die")]
public class CompleteDieAnimation : Action
{
    private MonsterView monsterView;
    private Animator _animator;

    [SerializeField] private SharedBool isDead;

    int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashUpper = Animator.StringToHash("Upper");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!isDead.Value) return TaskStatus.Failure;

        if (_animator.GetBool(hashIncapacitated))
        {
            if (_animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Running;
            else if (!_animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("Assassinated")) return TaskStatus.Running;
        }
        else
        {
            if (monsterView.IsAnimationRunning("Die"))
            {
                return TaskStatus.Running;
            }
        }

        return TaskStatus.Success;
    }    
}
